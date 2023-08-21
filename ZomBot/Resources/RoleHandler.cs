using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ZomBot.Data;

namespace ZomBot.Resources {
	public class RoleHandler {
		public static async Task CreateClanRole(SocketGuild guild, string clanName) {
			var guildAccount = Accounts.GetGuild(guild);

			if (guildAccount.clanList == null) {
				guildAccount.clanList = new List<Clan>();
				Accounts.SaveAccounts();
			}

			foreach (Clan c in guildAccount.clanList)
				if (c.clanName.ToLower().Contains(clanName.ToLower()))
					return;

			SocketRole curedRole = guild.GetRole(guildAccount.roleIDs.cured);

			var rand = new Random();
			uint color = (uint)rand.Next(0, 0xffffff);

			ulong r = (await guild.CreateRoleAsync(clanName, color: color, isHoisted: false, isMentionable: true)).Id;
			await guild.GetRole(r).ModifyAsync(x => x.Position = curedRole.Position + 1);

			guildAccount.clanList.Add(new Clan() {
				clanName = clanName,
				roleID = r
			});

			Console.WriteLine($"Created new role for clan: {clanName}");
			Accounts.SaveAccounts();
		}

		public static async Task CreateRoles(SocketGuild guild, bool endgame = false) {
			var guildAccount = Accounts.GetGuild(guild);
			bool updated = false;
			var roles = guild.Roles;

			{ // mod role
				var temprole = from r in roles where r.Id == guildAccount.roleIDs.mod select r;
				var role = temprole.FirstOrDefault();

				if (role == null) {
					guildAccount.roleIDs.mod = (await guild.CreateRoleAsync("Mod", color: Color.Blue, isHoisted: true, isMentionable: true)).Id;
					Console.WriteLine("Created new mod role.");
					updated = true;
				}
			}

			if (!endgame) { // mvz role
				var temprole = from r in roles where r.Id == guildAccount.roleIDs.mvz select r;
				var role = temprole.FirstOrDefault();

				if (role == null) {
					guildAccount.roleIDs.mvz = (await guild.CreateRoleAsync("MVZ", color: Color.DarkGreen, isHoisted: true, isMentionable: true)).Id;
					Console.WriteLine("Created new mvz role.");
					updated = true;
				}
			}

			if (!endgame) { // zombie role
				var temprole = from r in roles where r.Id == guildAccount.roleIDs.zombie select r;
				var role = temprole.FirstOrDefault();

				if (role == null) {
					guildAccount.roleIDs.zombie = (await guild.CreateRoleAsync("Zombie", color: Color.Green, isHoisted: false, isMentionable: true)).Id;
					Console.WriteLine("Created new zombie role.");
					updated = true;
				}
			}

			if (!endgame) { // cured role
				var temprole = from r in roles where r.Id == guildAccount.roleIDs.cured select r;
				var role = temprole.FirstOrDefault();

				if (role == null) {
					guildAccount.roleIDs.cured = (await guild.CreateRoleAsync("Cured", color: Color.Orange, isHoisted: false, isMentionable: true)).Id;
					Console.WriteLine("Created new revived role.");
					updated = true;
				}
			}

			if (!endgame) { // human role
				var temprole = from r in roles where r.Id == guildAccount.roleIDs.human select r;
				var role = temprole.FirstOrDefault();

				if (role == null) {
					guildAccount.roleIDs.human = (await guild.CreateRoleAsync("Human", color: Color.LightOrange, isHoisted: false, isMentionable: true)).Id;
					Console.WriteLine("Created new human role.");
					updated = true;
				}
			}

			{ // player role
				var temprole = from r in roles where r.Id == guildAccount.roleIDs.player select r;
				var role = temprole.FirstOrDefault();

				if (role == null) {
					guildAccount.roleIDs.player = (await guild.CreateRoleAsync("Player", color: Color.Default, isHoisted: false, isMentionable: true)).Id;
					Console.WriteLine("Created new player role.");
					updated = true;
				}
			}

			if (updated)
				Accounts.SaveAccounts();
		}

		private static async Task DeleteRoles(SocketGuild guild) {
			var guildAccount = Accounts.GetGuild(guild);
			var roles = guild.Roles;

			{ // mod -> veteran mod role
				var temprole = from r in roles where r.Id == guildAccount.roleIDs.mod select r;
				var role = temprole.FirstOrDefault();

				if (role != null) {
					guildAccount.roleIDs.mod = 0;
					await guild.GetRole(role.Id).ModifyAsync(x => x.Name = "Veteran Mod");
					Console.WriteLine("Updated mod role.");
				}
			}

			{ // mvz role
				var temprole = from r in roles where r.Id == guildAccount.roleIDs.mvz select r;
				var role = temprole.FirstOrDefault();

				if (role != null) {
					guildAccount.roleIDs.mvz = 0;
					await guild.GetRole(role.Id).DeleteAsync();
					Console.WriteLine("Deleted mvz role.");
				}
			}

			{ // zombie role
				var temprole = from r in roles where r.Id == guildAccount.roleIDs.zombie select r;
				var role = temprole.FirstOrDefault();

				if (role != null) {
					guildAccount.roleIDs.zombie = 0;
					await guild.GetRole(role.Id).DeleteAsync();
					Console.WriteLine("Deleted zombie role.");
				}
			}

			{ // human role
				var temprole = from r in roles where r.Id == guildAccount.roleIDs.human select r;
				var role = temprole.FirstOrDefault();

				if (role != null) {
					guildAccount.roleIDs.human = 0;
					await guild.GetRole(role.Id).DeleteAsync();
					Console.WriteLine("Deleted human role.");
				}
			}

			{ // cured role
				var temprole = from r in roles where r.Id == guildAccount.roleIDs.cured select r;
				var role = temprole.FirstOrDefault();

				if (role != null) {
					guildAccount.roleIDs.cured = 0;
					await guild.GetRole(role.Id).DeleteAsync();
					Console.WriteLine("Deleted cured role.");
				}
			}

			{ // player -> veteran role
				var temprole = from r in roles where r.Id == guildAccount.roleIDs.player select r;
				var role = temprole.FirstOrDefault();

				if (role != null) {
					guildAccount.roleIDs.player = 0;
					await guild.GetRole(role.Id).ModifyAsync(x => x.Name = "Veteran");
					Console.WriteLine("Updated player role.");
				}
			}

			foreach (Clan c in guildAccount.clanList) {
				var result = from r in roles
							 where r.Id == c.roleID
							 select r;

				var role = result.FirstOrDefault();
				if (role == null)
					continue;

				await role.DeleteAsync();
			}

			guildAccount.clanList = new List<Clan>();
			Accounts.SaveAccounts();
		}

		public static async Task JoinClan(IUser user, SocketGuild guild, string clanName) {
			await CreateClanRole(guild, clanName.Trim());
			var guildData = Accounts.GetGuild(guild.Id);
			var userButInGuild = guild.GetUser(user.Id);
			var userRoles = userButInGuild.Roles;
			var roles = guild.Roles;

			List<Clan> toRemove = new List<Clan>();
			foreach (Clan c in guildData.clanList) {
				var result = from r in roles
							 where r.Id == c.roleID
							 select r;

				var role = result.FirstOrDefault();
				if (role == null) {
					toRemove.Add(c);
					continue;
				}

				if (userRoles.Contains(role)) {
					if (c.clanName.Trim().ToLower() != clanName.Trim().ToLower()) {
						await userButInGuild.RemoveRoleAsync(role);
					}
				} else {
					if (c.clanName.Trim().ToLower() == clanName.Trim().ToLower()) {
						await userButInGuild.AddRoleAsync(role);
					}
				}
			}

			if (toRemove.Count() > 0) {
				foreach (Clan c in toRemove)
					guildData.clanList.Remove(c);

				Accounts.SaveAccounts();
			}
		}

		public static async Task LeaveClan(IUser user, SocketGuild guild) {
			var guildData = Accounts.GetGuild(guild.Id);
			var userButInGuild = guild.GetUser(user.Id);
			var userRoles = userButInGuild.Roles;
			var roles = guild.Roles;

			List<Clan> toRemove = new List<Clan>();
			foreach (Clan c in guildData.clanList) {
				var result = from r in roles
							 where r.Id == c.roleID
							 select r;

				var role = result.FirstOrDefault();
				if (role == null) {
					toRemove.Add(c);
					continue;
				}

				if (userRoles.Contains(role))
					await userButInGuild.RemoveRoleAsync(role);

				if (role.Members.Count() == 0) {
					await role.DeleteAsync();
					toRemove.Add(c);
				}
			}

			if (toRemove.Count() > 0) {
				foreach (Clan c in toRemove)
					guildData.clanList.Remove(c);

				Accounts.SaveAccounts();
			}
		}

		public static async Task JoinHumanTeam(IUser user, SocketGuild guild) {
			await CreateRoles(guild);
			var guildData = Accounts.GetGuild(guild.Id);
			var userData = Accounts.GetUser(user, guild);
			var userButInGuild = guild.GetUser(user.Id);
			bool addHuman = true, addPlayer = true, addCured = userData.specialPlayerData.cured;

			foreach (SocketRole role in userButInGuild.Roles) {
				if (role.Id == guildData.roleIDs.mod)
					await userButInGuild.RemoveRoleAsync(guildData.roleIDs.mod);
				else if (role.Id == guildData.roleIDs.mvz)
					await userButInGuild.RemoveRoleAsync(guildData.roleIDs.mvz);
				else if (role.Id == guildData.roleIDs.zombie)
					await userButInGuild.RemoveRoleAsync(guildData.roleIDs.zombie);
				else if (role.Id == guildData.roleIDs.human)
					addHuman = false;
				else if (role.Id == guildData.roleIDs.player)
					addPlayer = false;
				else if (role.Id == guildData.roleIDs.cured)
					addCured = false;
			}

			if (addHuman)
				await userButInGuild.AddRoleAsync(guildData.roleIDs.human);

			if (addPlayer)
				await userButInGuild.AddRoleAsync(guildData.roleIDs.player);

			if (addCured)
				await userButInGuild.AddRoleAsync(guildData.roleIDs.cured);
		}

		public static async Task JoinZombieTeam(IUser user, SocketGuild guild, bool isMVZ = false) {
			await CreateRoles (guild);
			var guildData = Accounts.GetGuild(guild.Id);
			var userButInGuild = guild.GetUser(user.Id);
			bool addZombie = true, addPlayer = true, addMVZ = true;

			foreach (SocketRole role in userButInGuild.Roles) {
				if (role.Id == guildData.roleIDs.mod)
					await userButInGuild.RemoveRoleAsync(guildData.roleIDs.mod);
				else if (role.Id == guildData.roleIDs.human)
					await userButInGuild.RemoveRoleAsync(guildData.roleIDs.human);
				else if (role.Id == guildData.roleIDs.cured)
					await userButInGuild.RemoveRoleAsync(guildData.roleIDs.cured);
				else if (role.Id == guildData.roleIDs.zombie)
					addZombie = false;
				else if (role.Id == guildData.roleIDs.player)
					addPlayer = false;
				else if (role.Id == guildData.roleIDs.mvz) {
					if (isMVZ)
						addMVZ = false;
					else
						await userButInGuild.RemoveRoleAsync(guildData.roleIDs.mvz);
				}
			}

			if (addZombie)
				await userButInGuild.AddRoleAsync(guildData.roleIDs.zombie);

			if (addPlayer)
				await userButInGuild.AddRoleAsync(guildData.roleIDs.player);

			if (addMVZ && isMVZ)
				await userButInGuild.AddRoleAsync(guildData.roleIDs.mvz);
		}

		public static async Task JoinModTeam(IUser user, SocketGuild guild) {
			await CreateRoles (guild);
			var guildData = Accounts.GetGuild(guild.Id);
			var userButInGuild = guild.GetUser(user.Id);
			bool addMod = true;

			foreach (SocketRole role in userButInGuild.Roles) {
				if (role.Id == guildData.roleIDs.mod)
					addMod = false;
				else if (role.Id == guildData.roleIDs.mvz)
					await userButInGuild.RemoveRoleAsync(guildData.roleIDs.mvz);
				else if (role.Id == guildData.roleIDs.zombie)
					await userButInGuild.RemoveRoleAsync(guildData.roleIDs.zombie);
				else if (role.Id == guildData.roleIDs.human)
					await userButInGuild.RemoveRoleAsync(guildData.roleIDs.human);
				else if (role.Id == guildData.roleIDs.cured)
					await userButInGuild.RemoveRoleAsync(guildData.roleIDs.cured);
				else if (role.Id == guildData.roleIDs.player)
					await userButInGuild.RemoveRoleAsync(guildData.roleIDs.player);
			}

			if (addMod)
				await userButInGuild.AddRoleAsync(guildData.roleIDs.mod);
		}

		public static async Task LeaveTeams(IUser user, SocketGuild guild) {
			await CreateRoles (guild);
			var guildData = Accounts.GetGuild(guild.Id);
			var userButInGuild = guild.GetUser(user.Id);

			foreach (SocketRole role in userButInGuild.Roles) {
				if (role.Id == guildData.roleIDs.mod)
					await userButInGuild.RemoveRoleAsync(guildData.roleIDs.mod);
				else if (role.Id == guildData.roleIDs.mvz)
					await userButInGuild.RemoveRoleAsync(guildData.roleIDs.mvz);
				else if (role.Id == guildData.roleIDs.zombie)
					await userButInGuild.RemoveRoleAsync(guildData.roleIDs.zombie);
				else if (role.Id == guildData.roleIDs.human)
					await userButInGuild.RemoveRoleAsync(guildData.roleIDs.human);
				else if (role.Id == guildData.roleIDs.player)
					await userButInGuild.RemoveRoleAsync(guildData.roleIDs.player);
			}
		}

		public static async void UpdateChannel(ulong channelID, SocketGuild guild) {
			await CreateRoles (guild);
			var guildAccount = Accounts.GetGuild(guild);
			var channel = guild.GetTextChannel(channelID);

			if (channel == null)
				return; // no channel to mess with

			OverwritePermissions permsNoSee = new OverwritePermissions(viewChannel: PermValue.Deny);
			OverwritePermissions permsSeeNoSpeak = new OverwritePermissions(viewChannel: PermValue.Allow, sendMessages: PermValue.Deny);
			OverwritePermissions permsSee = new OverwritePermissions(viewChannel: PermValue.Allow);
			OverwritePermissions permsManage = new OverwritePermissions(viewChannel: PermValue.Allow, manageMessages: PermValue.Allow);

			switch (guildAccount.channels.GetChannelType(channelID)) {
				case ChannelDesignation.MOD:
				case ChannelDesignation.MODIMPORTANT:
					await channel.AddPermissionOverwriteAsync(guild.GetRole(guildAccount.roleIDs.mod), permsManage);
					await channel.AddPermissionOverwriteAsync(guild.EveryoneRole, permsNoSee);
					return;
				case ChannelDesignation.LOG:
					await channel.AddPermissionOverwriteAsync(guild.EveryoneRole, permsNoSee);
					return;
				case ChannelDesignation.TAG:
					await channel.AddPermissionOverwriteAsync(guild.GetRole(guildAccount.roleIDs.mod), permsSeeNoSpeak);
					await channel.AddPermissionOverwriteAsync(guild.GetRole(guildAccount.roleIDs.human), permsSeeNoSpeak);
					await channel.AddPermissionOverwriteAsync(guild.GetRole(guildAccount.roleIDs.zombie), permsSeeNoSpeak);
					await channel.AddPermissionOverwriteAsync(guild.EveryoneRole, permsNoSee);
					return;
				case ChannelDesignation.HUMAN:
					await channel.AddPermissionOverwriteAsync(guild.GetRole(guildAccount.roleIDs.mod), permsManage);
					await channel.AddPermissionOverwriteAsync(guild.GetRole(guildAccount.roleIDs.human), permsSee);
					await channel.AddPermissionOverwriteAsync(guild.EveryoneRole, permsNoSee);
					return;
				case ChannelDesignation.HUMANANNOUNCEMENT:
					await channel.AddPermissionOverwriteAsync(guild.GetRole(guildAccount.roleIDs.mod), permsManage);
					await channel.AddPermissionOverwriteAsync(guild.GetRole(guildAccount.roleIDs.human), permsSeeNoSpeak);
					await channel.AddPermissionOverwriteAsync(guild.EveryoneRole, permsNoSee);
					return;
				case ChannelDesignation.ZOMBIE:
					await channel.AddPermissionOverwriteAsync(guild.GetRole(guildAccount.roleIDs.mod), permsManage);
					await channel.AddPermissionOverwriteAsync(guild.GetRole(guildAccount.roleIDs.zombie), permsSee);
					await channel.AddPermissionOverwriteAsync(guild.EveryoneRole, permsNoSee);
					return;
				case ChannelDesignation.ZOMBIEANNOUNCEMENT:
					await channel.AddPermissionOverwriteAsync(guild.GetRole(guildAccount.roleIDs.mod), permsManage);
					await channel.AddPermissionOverwriteAsync(guild.GetRole(guildAccount.roleIDs.zombie), permsSeeNoSpeak);
					await channel.AddPermissionOverwriteAsync(guild.EveryoneRole, permsNoSee);
					return;
				case ChannelDesignation.SHAREDANNOUNCEMENT:
					await channel.AddPermissionOverwriteAsync(guild.GetRole(guildAccount.roleIDs.mod), permsManage);
					await channel.AddPermissionOverwriteAsync(guild.GetRole(guildAccount.roleIDs.human), permsSeeNoSpeak);
					await channel.AddPermissionOverwriteAsync(guild.GetRole(guildAccount.roleIDs.zombie), permsSeeNoSpeak);
					await channel.AddPermissionOverwriteAsync(guild.EveryoneRole, permsNoSee);
					return;
				default: // null
					break;
			}
		}

		public static async Task EndGame(SocketGuild guild) {
			await CreateRoles(guild, true);
			var guildAccount = Accounts.GetGuild(guild);
			List<ulong> remove = new List<ulong>();

			OverwritePermissions permsNoSee = new OverwritePermissions(viewChannel: PermValue.Deny);
			OverwritePermissions permsSeeNoSpeak = new OverwritePermissions(viewChannel: PermValue.Allow, sendMessages: PermValue.Deny);
			OverwritePermissions permsSee = new OverwritePermissions(viewChannel: PermValue.Allow);
			OverwritePermissions permsManage = new OverwritePermissions(viewChannel: PermValue.Allow, manageMessages: PermValue.Allow);

			var guildTextChannels = guild.TextChannels;
			var roles = guild.Roles;
			IEnumerable<SocketRole> temprole;

			temprole = from r in roles where r.Id == guildAccount.roleIDs.mod select r;
			var modRole = temprole.FirstOrDefault();

			temprole = from r in roles where r.Id == guildAccount.roleIDs.player select r;
			var playerRole = temprole.FirstOrDefault();

			var everyoneRole = guild.EveryoneRole;


			foreach (Channel channel in guildAccount.channels.Get()) {
				var tempChannel = from c in guildTextChannels where c.Id == channel.id select c;
				var textChannel = tempChannel.FirstOrDefault();

				if (textChannel != null) {
					switch (channel.type) {
						case ChannelDesignation.MOD:
						case ChannelDesignation.MODIMPORTANT:
							await textChannel.AddPermissionOverwriteAsync(modRole, permsManage);
							await textChannel.AddPermissionOverwriteAsync(everyoneRole, permsNoSee);
							break;
						case ChannelDesignation.TAG:
						case ChannelDesignation.HUMAN:
						case ChannelDesignation.HUMANANNOUNCEMENT:
						case ChannelDesignation.ZOMBIE:
						case ChannelDesignation.ZOMBIEANNOUNCEMENT:
						case ChannelDesignation.SHAREDANNOUNCEMENT:
							await textChannel.AddPermissionOverwriteAsync(modRole, permsSeeNoSpeak);
							await textChannel.AddPermissionOverwriteAsync(playerRole, permsSeeNoSpeak);
							await textChannel.AddPermissionOverwriteAsync(everyoneRole, permsNoSee);
							break;
						default:
							break;
					}
				}
			}

			guildAccount.channels.Clear();
			
			// add postgame channels
			var category = await guild.CreateCategoryChannelAsync("Post Game");
			await category.AddPermissionOverwriteAsync(everyoneRole, permsNoSee);

			var pac = await guild.CreateTextChannelAsync("announcements");
			await pac.ModifyAsync(x => x.CategoryId = category.Id);
			await pac.AddPermissionOverwriteAsync(modRole, permsSee);
			await pac.AddPermissionOverwriteAsync(playerRole, permsSeeNoSpeak);
			await pac.AddPermissionOverwriteAsync(everyoneRole, permsNoSee);

			var cc = await guild.CreateTextChannelAsync("criticisms");
			await cc.ModifyAsync(x => x.CategoryId = category.Id);
			await cc.AddPermissionOverwriteAsync(modRole, permsSee);
			await cc.AddPermissionOverwriteAsync(playerRole, permsSee);
			await cc.AddPermissionOverwriteAsync(everyoneRole, permsNoSee);

			var ac = await guild.CreateTextChannelAsync("afterthoughts");
			await ac.ModifyAsync(x => x.CategoryId = category.Id);
			await ac.AddPermissionOverwriteAsync(modRole, permsSee);
			await ac.AddPermissionOverwriteAsync(playerRole, permsSee);
			await ac.AddPermissionOverwriteAsync(everyoneRole, permsNoSee);

			await pac.SendMessageAsync($"{playerRole.Mention} Please use {cc.Mention} and {ac.Mention} to leave your feedback of your experiences during this semester's HvZ!");
			await DeleteRoles(guild);
			Accounts.SaveAccounts();
		}
	}
}
