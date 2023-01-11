using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Timers;
using Discord;
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

        private static bool WebsiteStatus = false;

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
            _interactions = new InteractionHandler();

            await _handler.InitializeAsync(_client);
            await _interactions.InitializeAsync(_client);

            _client.Ready += Ready;
            //await _client.SetGameAsync("HvZ");
            _client.MessageUpdated += MessageUpdated;
            _client.MessageDeleted += MessageDeleted;

            Timer fiveMinutes = new Timer() {
                Interval = 300000,
                AutoReset = true,
                Enabled = true
            };

            if (Config.bot.apionline && Config.bot.apikey != "") {
                fiveMinutes.Elapsed += UpdatePlayers;
            }

            try {
                GetSiteData();
                await _client.SetGameAsync($"HVZ");
            } catch {
                await _client.SetGameAsync($"from the sidelines", null, ActivityType.Watching);
            };

            await Task.Delay(-1);
        }

        private async Task MessageDeleted(Cacheable<IMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel) {
            if (message.HasValue && channel.HasValue) {
                if (channel.Value is SocketGuildChannel c) {
                    var guildAccount = Accounts.GetGuild(c.Guild);
                    var logChannel = c.Guild.GetTextChannel(guildAccount.channels.logChannel);

                    if (logChannel == null) {
                        List<Overwrite> overwrites = new List<Overwrite> {
                            new Overwrite(c.Guild.EveryoneRole.Id, PermissionTarget.Role, new OverwritePermissions(viewChannel: PermValue.Deny))
                        };

                        ITextChannel ch = await c.Guild.CreateTextChannelAsync("message-log", x => x.PermissionOverwrites = overwrites);
                        guildAccount.channels.logChannel = ch.Id;
                        Accounts.SaveAccounts();
                        logChannel = (SocketTextChannel)ch;
                    }

                    EmbedBuilder embed = new EmbedBuilder();

                    embed.WithAuthor(message.Value.Author)
                         .AddField($"Deleted from #{c.Name}", message.Value.CleanContent)
                         .WithCurrentTimestamp();

                    await logChannel.SendMessageAsync(embed: embed.Build());
                }
            }
        }

        private async Task MessageUpdated(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel) {
            if (before.HasValue && after != null) {
                if (channel is SocketTextChannel c) {
                    var guildAccount = Accounts.GetGuild(c.Guild);
                    var logChannel = c.Guild.GetTextChannel(guildAccount.channels.logChannel);
                    if (logChannel == null) {
                        List<Overwrite> overwrites = new List<Overwrite> {
                            new Overwrite(c.Guild.EveryoneRole.Id, PermissionTarget.Role, new OverwritePermissions(viewChannel: PermValue.Deny))
                        };

                        ITextChannel ch = await c.Guild.CreateTextChannelAsync("message-log", x => x.PermissionOverwrites = overwrites);
                        guildAccount.channels.logChannel = ch.Id;
                        Accounts.SaveAccounts();
                        logChannel = (SocketTextChannel)ch;
                    }

                    EmbedBuilder embed = new EmbedBuilder();

                    embed.WithAuthor(after.Author)
                            .WithCurrentTimestamp()
                            .AddField($"Updated in: #{channel.Name}", before.Value.CleanContent)
                            .AddField("Changed to", after.CleanContent)
                            .WithCurrentTimestamp();

                    await logChannel.SendMessageAsync(embed: embed.Build());
                }
            }
        }

        private async Task Ready() {
            await _interactions.SetupAsync();
        }

        public static PlayerDataList getPDL() {
            return playerDataList;
        }

        public static ModDataList getMDL() {
            return modDataList;
        }

        public void GetSiteData() {
            string rawPlayerData = "";
            string rawModData = "";
            string rawMissionData = "";

            using (WebClient client = new WebClient()) {
                try {
                    rawPlayerData = client.DownloadString($"{Config.bot.hvzwebsite}/api/v2/status/players");
                    rawModData = client.DownloadString($"{Config.bot.hvzwebsite}/api/v2/status/moderators");
                    rawMissionData = client.DownloadString($"{Config.bot.hvzwebsite}/api/v2/admin/missions?apikey={Config.bot.apikey}");
                } catch (WebException) {
                    Log($"Could not retieve data from {Config.bot.hvzwebsite}");
                    throw new Exception($"Could not retieve data from {Config.bot.hvzwebsite}");
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
                    if (!g.gameActive)
                        continue;

                    int mvznum = 0;
                    int numZombies = 0;
                    foreach (PlayerData pl in playerDataList.players) {
                        if (pl.team != "zombie")
                            continue;

                        if (pl.humansTagged > mvznum) {
                            mvznum = pl.humansTagged;
                        }
                    }

                    await guild.DownloadUsersAsync();

                    if (guild.Users.Count > 0) {
                        foreach (SocketGuildUser user in guild.Users) {
                            if (user.IsBot)
                                continue;

                            var u = Accounts.GetUser(user, guild);

                            if (u.blacklisted)
                                continue;

                            if (u.playerData.name != null && u.playerData.name != "") {
                                foreach (PlayerData player in playerDataList.players) {
                                    if (u.playerData.id == player.id) {
                                        if (u.playerData.team == player.team && u.playerData.clan == player.clan)
                                            if (u.playerData.team == "human")
                                                break;

                                        updatedPlayers++;
                                        u.playerData = player;
                                        Accounts.SaveAccounts();

                                        if (u.playerData.team == "human") {
                                            await RoleHandler.JoinHumanTeam(user, guild);

                                            if (u.playerData.clan != null && u.playerData.clan != "")
                                                await RoleHandler.JoinClan(user, guild, u.playerData.clan);
                                            else
                                                await RoleHandler.LeaveClan(user, guild);
                                        } else if (u.playerData.team == "zombie") {
                                            numZombies++;
                                            await RoleHandler.JoinZombieTeam(user, guild, (mvznum > 0 && u.playerData.humansTagged >= mvznum));
                                            await RoleHandler.LeaveClan(user, guild);
                                        }

                                        break;
                                    }
                                }

                                foreach (PlayerData mod in modDataList.players) {
                                    if (u.playerData.id == mod.id) {
                                        if (u.playerData.team == mod.team)
                                            break;

                                        updatedPlayers++;

                                        u.playerData = mod;
                                        Accounts.SaveAccounts();
                                        await RoleHandler.JoinModTeam(user, guild);
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    if (g.missions.missions == null)
                        g.missions.missions = new List<MissionData>();

                    if (missionList.missions != null) {
                        foreach (MissionData m in missionList.missions) {
                            bool cont = false;
                            foreach (MissionData saved in g.missions.missions)
                                if (m.id == saved.id)
                                    cont = true;

                            if (cont)
                                continue;

                            string b = Resources.Formatting.HtmlToPlainText(m.body);

                            if (m.team == "human") {
                                await guild.GetTextChannel(g.channels.humanAnnouncementChannel).SendMessageAsync($"**{m.title}**");
                                await guild.GetTextChannel(g.channels.humanAnnouncementChannel).SendMessageAsync(b);
                                await guild.GetTextChannel(g.channels.humanAnnouncementChannel).SendMessageAsync(guild.GetRole(g.roleIDs.human).Mention);
                            } else if (m.team == "zombie") {
                                await guild.GetTextChannel(g.channels.zombieAnnouncementChannel).SendMessageAsync($"**{m.title}**");
                                await guild.GetTextChannel(g.channels.zombieAnnouncementChannel).SendMessageAsync(b);
                                await guild.GetTextChannel(g.channels.zombieAnnouncementChannel).SendMessageAsync(guild.GetRole(g.roleIDs.zombie).Mention);
                            } else if (m.team == "all") {
                                await guild.GetTextChannel(g.channels.generalAnnouncementChannel).SendMessageAsync($"**{m.title}**");
                                await guild.GetTextChannel(g.channels.generalAnnouncementChannel).SendMessageAsync(b);
                                await guild.GetTextChannel(g.channels.generalAnnouncementChannel).SendMessageAsync(guild.GetRole(g.roleIDs.player).Mention);
                            }

                            MissionData d = new MissionData() {
                                id = m.id,
                                team = m.team,
                                title = m.title,
                                body = b
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

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg);
            return Task.CompletedTask;
        }

        private void Log(string msg) {
            Console.WriteLine($"[{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}] {msg}");
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
