using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Timers;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Newtonsoft.Json;
using ZomBot.Data;
using ZomBot.Resources;

namespace ZomBot {
	public class Program {
		DiscordSocketClient _client;
		CommandHandler _handler;
		InteractionHandler _interactions;

		private static PlayerDataList playerDataList;
		private static ModDataList modDataList;
		private static MissionList missionList;

		public static bool websiteDown = false;
		public static bool overrideCheck = false;

		static void Main(string[] args) => new Program().StartAsync().GetAwaiter().GetResult();

		public async Task StartAsync() {
			if (Config.bot.token == "" || Config.bot.token == null || Config.bot.token == "insert bot token here") {
				Console.WriteLine("Insert bot token into config.json");
				return;
			}

			GatewayIntents intents = GatewayIntents.All;
			intents -= GatewayIntents.GuildInvites;
			intents -= GatewayIntents.GuildScheduledEvents;
			intents -= GatewayIntents.GuildPresences;

			_client = new DiscordSocketClient(new DiscordSocketConfig {
				LogLevel = LogSeverity.Warning,
				GatewayIntents = intents,
				MessageCacheSize = Config.bot.cachesize
			});

			_client.Log += Log;

			await _client.LoginAsync(TokenType.Bot, Config.bot.token);
			await _client.StartAsync();

			_handler = new CommandHandler();
			_interactions = new InteractionHandler(_client);

			await _handler.InitializeAsync(_client);
			await _interactions.InitializeAsync();

			_client.Ready += Ready;
			//await _client.SetGameAsync("HvZ");
			_client.MessageUpdated += MessageUpdated;
			_client.MessageDeleted += MessageDeleted;

			Timer t = new Timer() {
				Interval = 60000, // 1 minute
				AutoReset = true,
				Enabled = true
			};

			if (Config.bot.apionline && Config.bot.apikey != "") {
				Info("API ONLINE");
				t.Elapsed += UpdatePlayers;

				try {
					GetSiteData();
				} catch {
					await _client.SetGameAsync($"from the sidelines", null, ActivityType.Watching);
				};
			} else {
				await _client.SetGameAsync($"with myself...", null, ActivityType.Playing);
				Info("API OFFLINE");
			}

			await Task.Delay(-1);
		}

		private async Task MessageDeleted(Cacheable<IMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel) {
			if (message.HasValue && channel.HasValue) {
				if (message.Value.Author.IsBot || (message.Value?.CleanContent?.Trim() ?? "") == "")
					return;

				if (channel.Value is SocketGuildChannel c) {
					var guildAccount = Accounts.GetGuild(c.Guild);
					SocketTextChannel logTextChannel = c.Guild.GetTextChannel(guildAccount.channels.GetFirstChannelByType(ChannelDesignation.LOG) ?? 0);

					if (logTextChannel == null) {
						List<Overwrite> overwrites = new List<Overwrite> {
							new Overwrite(c.Guild.EveryoneRole.Id, PermissionTarget.Role, new OverwritePermissions(viewChannel: PermValue.Deny))
						};

						RestTextChannel textChannel = await c.Guild.CreateTextChannelAsync("message-log", x => x.PermissionOverwrites = overwrites);
						guildAccount.channels.AddUnique(textChannel.Id, ChannelDesignation.LOG);
						Accounts.SaveAccounts();
						logTextChannel = c.Guild.GetTextChannel(textChannel.Id);
					}

					var chatLog = ChatManager.GetChatLog(message.Value.Author);
					chatLog.DeleteMessage(message.Value.Id);
					ChatManager.SaveChatLogs();

					try {
						EmbedBuilder embed = new EmbedBuilder();

						embed.WithAuthor(message.Value.Author)
							 .AddField($"Deleted from #{c.Name}", (message.Value.CleanContent ?? "") == "" ? "no text" : message.Value.CleanContent)
							 .WithCurrentTimestamp();

						await logTextChannel.SendMessageAsync(embed: embed.Build());
					} catch {
						Error("Couldn't log MessageDelete");
					}
				}
			}
		}

		private async Task MessageUpdated(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel) {
			if (before.HasValue && after != null) {
				if ((before.Value?.Author?.IsBot ?? false) || (before.Value?.CleanContent?.Trim() ?? "") == "")
					return;

				if (before.Value.CleanContent != after.CleanContent) {
					if (channel is SocketTextChannel c) {
						var guildAccount = Accounts.GetGuild(c.Guild);
						SocketTextChannel logTextChannel = c.Guild.GetTextChannel(guildAccount.channels.GetFirstChannelByType(ChannelDesignation.LOG) ?? 0);

						if (logTextChannel == null) {
							List<Overwrite> overwrites = new List<Overwrite> {
								new Overwrite(c.Guild.EveryoneRole.Id, PermissionTarget.Role, new OverwritePermissions(viewChannel: PermValue.Deny))
							};

							RestTextChannel textChannel = await c.Guild.CreateTextChannelAsync("message-log", x => x.PermissionOverwrites = overwrites);
							guildAccount.channels.AddUnique(textChannel.Id, ChannelDesignation.LOG);
							Accounts.SaveAccounts();
							logTextChannel = c.Guild.GetTextChannel(textChannel.Id);
						}

						var chatLog = ChatManager.GetChatLog(before.Value.Author);
						chatLog.EditMessage(before.Value.Id, after.CleanContent);
						ChatManager.SaveChatLogs();

						try {
							EmbedBuilder embed = new EmbedBuilder();

							var b = before.Value.CleanContent ?? "";
							var a = after.CleanContent ?? "";

							var bButSplit = b.Split(' ');
							var aButSplit = a.Split(' ');

							bool changed = false;

							if ((aButSplit?.Length ?? 0) > 0 && (bButSplit?.Length ?? 0) > 0) { // make sure no messages are empty
								for (int i = 0; i < bButSplit.Length; i++) {
									if (i >= aButSplit.Length) // after is shorter than before, avoid array index out of bounds
										break;
										
									if (bButSplit[i] == aButSplit[i]) {
										if (changed) {
											aButSplit[i - 1] = $"{aButSplit[i - 1]}**";
											changed = false;
										}
									} else {
										if (!changed) {
											aButSplit[i] = $"**{aButSplit[i]}";
											changed = true;
										}
									}
								}

								if (aButSplit.Length > bButSplit.Length) { // after is longer than before
									if (!changed) { // no ongoing boldening, so we start at then end of before
										aButSplit[bButSplit.Length] = $"**{aButSplit[bButSplit.Length]}";
										changed = true;
									}
								}
							}

							var aAfterBolding = string.Join(" ", aButSplit);
							if (changed)
								aAfterBolding += "**"; // tie up loose ends

							embed.WithAuthor(after.Author)
								.WithCurrentTimestamp()
								.AddField($"Updated in: #{channel.Name}", b == "" ? "no text" : b)
								.AddField("Changed to", aAfterBolding == "" ? "no text" : aAfterBolding)
								.WithCurrentTimestamp();

							await logTextChannel.SendMessageAsync(embed: embed.Build());
						} catch {
							Error("Couldn't log MessageUpdated");
						}
					}
				}
			}
		}

		private async Task Ready() {
			await _interactions.SetupAsync();
		}

		public static PlayerDataList GetPDL() {
			return playerDataList;
		}

		public static ModDataList GetMDL() {
			return modDataList;
		}

		public static void GetSiteData() {
			string rawPlayerData = "";
			string rawModData = "";
			string rawMissionData = "";

			using (WebClient client = new WebClient()) {
				try {
					rawPlayerData = client.DownloadString($"{Config.bot.hvzwebsite}/api/v2/status/players");
					rawModData = client.DownloadString($"{Config.bot.hvzwebsite}/api/v2/status/moderators");
					rawMissionData = client.DownloadString($"{Config.bot.hvzwebsite}/api/v2/admin/missions?apikey={Config.bot.apikey}");
					
					if (websiteDown) {
						websiteDown = false;
						Info("Website back online.");
					}
				} catch (WebException) {
					if (!websiteDown) {
						websiteDown = true;
						Warning($"Could not retieve data from {Config.bot.hvzwebsite}");
						throw new Exception($"Could not retieve data from {Config.bot.hvzwebsite}");
					}
				}
			}

			playerDataList = JsonConvert.DeserializeObject<PlayerDataList>(rawPlayerData);
			modDataList = JsonConvert.DeserializeObject<ModDataList>(rawModData);
			missionList = JsonConvert.DeserializeObject<MissionList>(rawMissionData);
		}
		private async void UpdatePlayers(object sender, ElapsedEventArgs e) {
			try {
				GetSiteData();
			} catch {
				await _client.SetGameAsync($"from the sidelines", null, ActivityType.Watching);
				return;
			}

			int updatedPlayers = 0;

			if (_client.Guilds.Count > 0) {
				foreach (SocketGuild guild in _client.Guilds) {
					var g = Accounts.GetGuild(guild);

					int mvznum = 0;
					int numZombies = 0;
					foreach (PlayerData pl in playerDataList.players) { // update needed count for mvz
						if (pl.team != "zombie")
							continue;

						if (pl.humansTagged > mvznum)
							mvznum = pl.humansTagged;
					}

					await guild.DownloadUsersAsync();

					if (guild.Users.Count > 0) {
						foreach (SocketGuildUser user in guild.Users) {
							if (user.IsBot)
								continue;

							var u = Accounts.GetUser(user, guild);

							if (u.blacklisted)
								continue;

							if ((u.playerData?.name ?? "") != "") { // player is in fact linked
								foreach (PlayerData player in playerDataList.players) { // iterate through non-mod players
									if (u.playerData.id == player.id) { // found player
										if (!overrideCheck && u.playerData.team == player.team && player.team == "human" && u.playerData.clan == player.clan && u.playerData.access == player.access)
												break; // nothing changed, continue to next user

										bool updated = false;
										var tagChannel = guild.GetTextChannel(g.channels.GetFirstChannelByType(ChannelDesignation.TAG) ?? 0);

										if (u.playerData.team == "zombie" && player.team == "human") { // changed from zombie to human
											u.specialPlayerData.cured = true;
											updated = true;
											Log($"{u.playerData.name} was cured.");
											
											if (tagChannel != null)
												await tagChannel.SendMessageAsync($"{user.DisplayName} ({u.playerData.name}) was cured!");
										}

										if (u.playerData.team == "human" && player.team == "zombie") { // infected human
											updated = true;
											Log($"{u.playerData.name} was tagged.");

											if (tagChannel != null)
												await tagChannel.SendMessageAsync($"{user.DisplayName} ({u.playerData.name}) was tagged!");
										}

										if ((u.playerData?.clan ?? "") != player.clan || (u.playerData?.humansTagged ?? 0) != player.humansTagged)
											updated = true;

										u.playerData = player;

										if (u.playerData.team == "human") {
											await RoleUtils.JoinHumanTeam(user, guild);

											if ((u.playerData?.clan ?? "") != "")
												await RoleUtils.JoinClan(user, guild, u.playerData.clan);
											else
												await RoleUtils.LeaveClan(user, guild);
										} else if (u.playerData.team == "zombie") {
											numZombies++;
											if (u.specialPlayerData.cured) {
												u.specialPlayerData.cured = false;
												updated = true;
											}

											bool isMVZ = mvznum > 0 && u.playerData.humansTagged >= mvznum;

											if (isMVZ && !u.specialPlayerData.isMVZ) {
												u.specialPlayerData.isMVZ = isMVZ;
												updated = true;
											}

											await RoleUtils.JoinZombieTeam(user, guild, isMVZ);
											await RoleUtils.LeaveClan(user, guild); // zombies don't have affiliations with anyone but other zombies
										}

										if (updated)
											updatedPlayers++;

										Accounts.SaveAccounts();
										break; // played found & updated, next player
									}
								}

								foreach (PlayerData mod in modDataList.players) { // iterate through mods
									if (u.playerData.id == mod.id) {
										if (u.playerData.access == mod.access)
											break;

										updatedPlayers++;

										u.playerData = mod;
										Accounts.SaveAccounts();
										await RoleUtils.JoinModTeam(user, guild);
										break; // player updated, next player
									}
								}
							}
						}

						if (overrideCheck) {
							overrideCheck = false;
							Info("Finished overrideCheck!");
						}
					}

					if (g.missions.missions == null)
						g.missions.missions = new List<MissionData>();

					if (missionList.missions != null) {
						foreach (MissionData m in missionList.missions) {
							bool cont = false;
							foreach (MissionData saved in g.missions.missions)
								if (m.id == saved.id) {
									cont = true;
									break;
								}

							if (cont)
								continue;

							if (m.GetPostDate().ToUnixTimeMilliseconds() > DateTimeOffset.Now.ToUnixTimeMilliseconds())
								continue; // not right time to post

							string b = Resources.Formatting.HtmlToPlainText(m.body).Replace("  ", " ");
							
							if (m.team == "human") {
								var humanAnnouncementChannel = guild.GetTextChannel(g.channels.GetFirstChannelByType(ChannelDesignation.HUMANANNOUNCEMENT) ?? 0);

								if (humanAnnouncementChannel != null) {
									await humanAnnouncementChannel.SendMessageAsync($"# {m.title} #\n{b}\n" + guild.GetRole(g.roleIDs.human).Mention);
								}
							} else if (m.team == "zombie") {
								var zombieAnnouncementChannel = guild.GetTextChannel(g.channels.GetFirstChannelByType(ChannelDesignation.ZOMBIEANNOUNCEMENT) ?? 0);

								if (zombieAnnouncementChannel != null) {
									await zombieAnnouncementChannel.SendMessageAsync($"# {m.title} #\n{b}\n" + guild.GetRole(g.roleIDs.zombie).Mention);
								}
							} else if (m.team == "all") {
								var sharedAnnouncementChannel = guild.GetTextChannel(g.channels.GetFirstChannelByType(ChannelDesignation.SHAREDANNOUNCEMENT) ?? 0);

								if (sharedAnnouncementChannel != null) {
									await sharedAnnouncementChannel.SendMessageAsync($"# {m.title} #\n{b}\n" + guild.GetRole(g.roleIDs.player).Mention);
								}
							}

							MissionData d = new MissionData() {
								id = m.id,
								team = m.team,
								title = m.title,
								body = b,
								postDate = m.GetPostDate().ToString()
							};

							g.missions.missions.Add(d);
							Accounts.SaveAccounts();
						}
					}

					await _client.SetGameAsync($"with {playerDataList.total - numZombies}h v {numZombies}z");
				}
			}

			if (updatedPlayers > 0)
				Log($"Updated {updatedPlayers}/{playerDataList.total + modDataList.players.Count} people.");
		}

		private Task Log(LogMessage msg) {
			if (msg.Exception is GatewayReconnectException)
				Log("Reconnect");
			else
				switch(msg.Severity) {
					case LogSeverity.Info:
						Info(msg.Message);
						break;
					case LogSeverity.Warning:
						Warning(msg.Message);
						break;
					case LogSeverity.Error:
						Error(msg.Message);
						break;
					case LogSeverity.Critical:
						Critical(msg.Message);
						break;
					default:
						Log("[???] " + msg.Message);
						break;
				}
			return Task.CompletedTask;
		}

		private static void Log(string msg) {
			Console.WriteLine($"[{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}] {msg}");
		}

		public static void Info(string msg) {
			Log("[Info] " + msg);
		}

		public static void Warning(string msg) {
			Log("[Warning] " + msg);
		}

		public static void Error(string msg) {
			Log("[Error] " + msg);
		}

		public static void Critical(string msg) {
			Log("[Critical] " + msg);
		}
	}

	public struct PlayerDataList {
		public List<PlayerData> players;
		public int total;
	}

	public struct ModDataList {
		public List<PlayerData> players;
	}
}

// convert linking as a mod to require the human/zombie id of that person
// make human/zombie id accessible in discord
// add badges to discord?