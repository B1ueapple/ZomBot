using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
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

			SocketRole humanRole = guild.GetRole(guildAccount.roleIDs.human);

			var rand = new Random();
			uint color = (uint)rand.Next(0, 0xffffff);

			ulong r = (await guild.CreateRoleAsync(clanName, color: color, isHoisted: false, isMentionable: true)).Id;
			await guild.GetRole(r).ModifyAsync(x => x.Position = humanRole.Position + 1);

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

			if (!endgame) { // mod role
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

			{ // veteranmod role
				var temprole = from r in roles where r.Id == guildAccount.roleIDs.veteranmod select r;
				var role = temprole.FirstOrDefault();

				if (guild.GetRole(guildAccount.roleIDs.veteranmod) == null) {
					guildAccount.roleIDs.veteranmod = (await guild.CreateRoleAsync("Veteran Mod", color: Color.DarkBlue, isHoisted: true, isMentionable: true)).Id;
					Console.WriteLine("Created new veteranmod role.");
					updated = true;
				}
			}

			{ // survivor role
				var temprole = from r in roles where r.Id == guildAccount.roleIDs.survivor select r;
				var role = temprole.FirstOrDefault();

				if (role == null) {
					guildAccount.roleIDs.survivor = (await guild.CreateRoleAsync("Survivor", color: Color.Magenta, isHoisted: true, isMentionable: true)).Id;
					Console.WriteLine("Created new survivor role.");
					updated = true;
				}
			}

			if (!endgame) { // player role
				var temprole = from r in roles where r.Id == guildAccount.roleIDs.player select r;
				var role = temprole.FirstOrDefault();

				if (role == null) {
					guildAccount.roleIDs.player = (await guild.CreateRoleAsync("Player", color: Color.Default, isHoisted: false, isMentionable: true)).Id;
					Console.WriteLine("Created new player role.");
					updated = true;
				}
			}

			{ // veteran role
				var temprole = from r in roles where r.Id == guildAccount.roleIDs.veteran select r;
				var role = temprole.FirstOrDefault();

				if (role == null) {
					guildAccount.roleIDs.veteran = (await guild.CreateRoleAsync("Veteran", color: Color.Teal, isHoisted: false, isMentionable: true)).Id;
					Console.WriteLine("Created new veteran role.");
					updated = true;
				}
			}

			if (updated)
				Accounts.SaveAccounts();
		}

		private static async Task DeleteRoles(SocketGuild guild) {
			var guildAccount = Accounts.GetGuild(guild);
			var roles = guild.Roles;

			{ // mod role
				var temprole = from r in roles where r.Id == guildAccount.roleIDs.mod select r;
				var role = temprole.FirstOrDefault();

				if (role != null) {
					guildAccount.roleIDs.mod = 0;
					await guild.GetRole(role.Id).DeleteAsync();
					Console.WriteLine("Deleted mod role.");
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
					Console.WriteLine("Deleted revived role.");
				}
			}

			{ // player role
				var temprole = from r in roles where r.Id == guildAccount.roleIDs.player select r;
				var role = temprole.FirstOrDefault();

				if (role != null) {
					guildAccount.roleIDs.player = 0;
					await guild.GetRole(role.Id).DeleteAsync();
					Console.WriteLine("Deleted player role.");
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
			await CreateClanRole(guild, clanName);
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
					if (c.clanName.ToLower() != clanName.ToLower()) {
						await userButInGuild.RemoveRoleAsync(role);
					}
				} else {
					if (c.clanName.ToLower() == clanName.ToLower()) {
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

		public static async Task LeaveClan(IUser user, SocketGuild guild, bool zombified = false) {
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

					if (zombified)
						guildData.gameLog.EventMessage(GameLogEvents.CLANWIPED, clan: c.clanName);
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
				else if (role.Id == guildData.roleIDs.veteran)
					await userButInGuild.RemoveRoleAsync(guildData.roleIDs.veteran);
				else if (role.Id == guildData.roleIDs.veteranmod)
					await userButInGuild.RemoveRoleAsync(guildData.roleIDs.veteranmod);
				else if (role.Id == guildData.roleIDs.survivor)
					await userButInGuild.RemoveRoleAsync(guildData.roleIDs.survivor);
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
				else if (role.Id == guildData.roleIDs.veteran)
					await userButInGuild.RemoveRoleAsync(guildData.roleIDs.veteran);
				else if (role.Id == guildData.roleIDs.veteranmod)
					await userButInGuild.RemoveRoleAsync(guildData.roleIDs.veteranmod);
				else if (role.Id == guildData.roleIDs.survivor)
					await userButInGuild.RemoveRoleAsync(guildData.roleIDs.survivor);
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
				else if (role.Id == guildData.roleIDs.veteran)
					await userButInGuild.RemoveRoleAsync(guildData.roleIDs.veteran);
				else if (role.Id == guildData.roleIDs.veteranmod)
					await userButInGuild.RemoveRoleAsync(guildData.roleIDs.veteranmod);
				else if (role.Id == guildData.roleIDs.survivor)
					await userButInGuild.RemoveRoleAsync(guildData.roleIDs.survivor);
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
			List<ulong> remove = new List<ulong>();

			OverwritePermissions permsNoSee = new OverwritePermissions(viewChannel: PermValue.Deny);
			OverwritePermissions permsSeeNoSpeak = new OverwritePermissions(viewChannel: PermValue.Allow, sendMessages: PermValue.Deny);
			OverwritePermissions permsSee = new OverwritePermissions(viewChannel: PermValue.Allow);

			if (channelID == guildAccount.channels.generalAnnouncementChannel) { // general announcement channel check
				var channel = guild.GetTextChannel(channelID);

				if (channel != null) {
					await channel.AddPermissionOverwriteAsync(guild.GetRole(guildAccount.roleIDs.mod), permsSee);
					await channel.AddPermissionOverwriteAsync(guild.GetRole(guildAccount.roleIDs.human), permsSeeNoSpeak);
					await channel.AddPermissionOverwriteAsync(guild.GetRole(guildAccount.roleIDs.zombie), permsSeeNoSpeak);
					await channel.AddPermissionOverwriteAsync(guild.EveryoneRole, permsNoSee);
					return;
				}
			}

			if (channelID == guildAccount.channels.humanAnnouncementChannel) { // human announcement channel check
				var channel = guild.GetTextChannel(channelID);

				if (channel != null) {
					await channel.AddPermissionOverwriteAsync(guild.GetRole(guildAccount.roleIDs.mod), permsSee);
					await channel.AddPermissionOverwriteAsync(guild.GetRole(guildAccount.roleIDs.human), permsSeeNoSpeak);
					await channel.AddPermissionOverwriteAsync(guild.EveryoneRole, permsNoSee);
					return;
				}
			}

			if (channelID == guildAccount.channels.zombieAnnouncementChannel) { // zombie announcement channel check
				var channel = guild.GetTextChannel(channelID);

				if (channel != null) {
					await channel.AddPermissionOverwriteAsync(guild.GetRole(guildAccount.roleIDs.mod), permsSee);
					await channel.AddPermissionOverwriteAsync(guild.GetRole(guildAccount.roleIDs.zombie), permsSeeNoSpeak);
					await channel.AddPermissionOverwriteAsync(guild.EveryoneRole, permsNoSee);
					return;
				}
			}

			{ // Check mod channels
				var result = from a in guildAccount.channels.modChannels
							 where a == channelID
							 select a;

				var channel = guild.GetTextChannel(result.FirstOrDefault());

				if (channel != null) {
					await channel.AddPermissionOverwriteAsync(guild.GetRole(guildAccount.roleIDs.mod), permsSee);
					await channel.AddPermissionOverwriteAsync(guild.EveryoneRole, permsNoSee);
					return;
				}
			}

			{ // Check human channels
				var result = from a in guildAccount.channels.humanChannels
							 where a == channelID
							 select a;

				var channel = guild.GetTextChannel(result.FirstOrDefault());

				if (channel != null) {
					await channel.AddPermissionOverwriteAsync(guild.GetRole(guildAccount.roleIDs.mod), permsSee);
					await channel.AddPermissionOverwriteAsync(guild.GetRole(guildAccount.roleIDs.human), permsSee);
					await channel.AddPermissionOverwriteAsync(guild.EveryoneRole, permsNoSee);
					return;
				}
			}

			{ // Check zombie channels
				var result = from a in guildAccount.channels.zombieChannels
							 where a == channelID
							 select a;

				var channel = guild.GetTextChannel(result.FirstOrDefault());

				if (channel != null) {
					await channel.AddPermissionOverwriteAsync(guild.GetRole(guildAccount.roleIDs.mod), permsSee);
					await channel.AddPermissionOverwriteAsync(guild.GetRole(guildAccount.roleIDs.zombie), permsSee);
					await channel.AddPermissionOverwriteAsync(guild.EveryoneRole, permsNoSee);
					return;
				}
			}
		}

		public static async Task EndGame(SocketGuild guild, bool survivors) {
			await CreateRoles(guild, true);
			var guildAccount = Accounts.GetGuild(guild);
			List<ulong> remove = new List<ulong>();

			OverwritePermissions permsNoSee = new OverwritePermissions(viewChannel: PermValue.Deny);
			OverwritePermissions permsSeeNoSpeak = new OverwritePermissions(viewChannel: PermValue.Allow, sendMessages: PermValue.Deny);
			OverwritePermissions permsSee = new OverwritePermissions(viewChannel: PermValue.Allow);

			var channels = guild.TextChannels;
			var roles = guild.Roles;
			IEnumerable<SocketRole> temprole;

			temprole = from r in roles where r.Id == guildAccount.roleIDs.survivor select r;
			var survivorrole = temprole.FirstOrDefault();

			temprole = from r in roles where r.Id == guildAccount.roleIDs.human select r;
			var humanrole = temprole.FirstOrDefault();

			temprole = from r in roles where r.Id == guildAccount.roleIDs.veteranmod select r;
			var vetmodrole = temprole.FirstOrDefault();

			temprole = from r in roles where r.Id == guildAccount.roleIDs.mod select r;
			var modrole = temprole.FirstOrDefault();

			temprole = from r in roles where r.Id == guildAccount.roleIDs.veteran select r;
			var vetrole = temprole.FirstOrDefault();

			temprole = from r in roles where r.Id == guildAccount.roleIDs.player select r;
			var playerrole = temprole.FirstOrDefault();

			// update all game channels for post game roles
			{
				var tempchannel = from c in channels where c.Id == guildAccount.channels.generalAnnouncementChannel select c;
				var channel = tempchannel.FirstOrDefault();

				if (channel != null) {
					await channel.AddPermissionOverwriteAsync(vetmodrole, permsSeeNoSpeak);
					await channel.AddPermissionOverwriteAsync(vetrole, permsSeeNoSpeak);
					await channel.AddPermissionOverwriteAsync(guild.EveryoneRole, permsNoSee);
				}
			}

			{
				var tempchannel = from c in channels where c.Id == guildAccount.channels.humanAnnouncementChannel select c;
				var channel = tempchannel.FirstOrDefault();

				if (channel != null) {
					await channel.AddPermissionOverwriteAsync(vetmodrole, permsSeeNoSpeak);
					await channel.AddPermissionOverwriteAsync(vetrole, permsSeeNoSpeak);
					await channel.AddPermissionOverwriteAsync(guild.EveryoneRole, permsNoSee);
				}
			}

			{
				var tempchannel = from c in channels where c.Id == guildAccount.channels.zombieAnnouncementChannel select c;
				var channel = tempchannel.FirstOrDefault();

				if (channel != null) {
					await channel.AddPermissionOverwriteAsync(vetmodrole, permsSeeNoSpeak);
					await channel.AddPermissionOverwriteAsync(vetrole, permsSeeNoSpeak);
					await channel.AddPermissionOverwriteAsync(guild.EveryoneRole, permsNoSee);
				}
			}

			foreach (ulong channelID in guildAccount.channels.modChannels) {
				var tempchannel = from c in channels
							 where c.Id == channelID
							 select c;

				var channel = tempchannel.FirstOrDefault();
				if (channel == null)
					continue;

				if (channel == null)
					continue;

				await channel.AddPermissionOverwriteAsync(vetmodrole, permsSee);
				await channel.AddPermissionOverwriteAsync(guild.EveryoneRole, permsNoSee);
			}

			foreach (ulong channelID in guildAccount.channels.humanChannels) {
				var tempchannel = from c in channels
								  where c.Id == channelID
								  select c;

				var channel = tempchannel.FirstOrDefault();
				if (channel == null)
					continue;

				if (channel == null)
					continue;

				await channel.AddPermissionOverwriteAsync(vetmodrole, permsSeeNoSpeak);
				await channel.AddPermissionOverwriteAsync(vetrole, permsSeeNoSpeak);
				await channel.AddPermissionOverwriteAsync(guild.EveryoneRole, permsNoSee);
			}

			foreach (ulong channelID in guildAccount.channels.zombieChannels) {
				var tempchannel = from c in channels
								  where c.Id == channelID
								  select c;

				var channel = tempchannel.FirstOrDefault();
				if (channel == null)
					continue;

				if (channel == null)
					continue;

				await channel.AddPermissionOverwriteAsync(vetmodrole, permsSeeNoSpeak);
				await channel.AddPermissionOverwriteAsync(vetrole, permsSeeNoSpeak);
				await channel.AddPermissionOverwriteAsync(guild.EveryoneRole, permsNoSee);
			}

			guildAccount.channels.generalAnnouncementChannel = 0;
			guildAccount.channels.humanAnnouncementChannel = 0;
			guildAccount.channels.zombieAnnouncementChannel = 0;
			guildAccount.channels.modChannels = new List<ulong>();
			guildAccount.channels.humanChannels = new List<ulong>();
			guildAccount.channels.zombieChannels = new List<ulong>();
			
			// add postgame channels
			var category = await guild.CreateCategoryChannelAsync("Post Game");
			await category.AddPermissionOverwriteAsync(guild.EveryoneRole, permsNoSee);

			var pac = await guild.CreateTextChannelAsync("announcements");
			await pac.ModifyAsync(x => x.CategoryId = category.Id);
			await pac.AddPermissionOverwriteAsync(vetmodrole, permsSee);
			await pac.AddPermissionOverwriteAsync(vetrole, permsSeeNoSpeak);
			await pac.AddPermissionOverwriteAsync(guild.EveryoneRole, permsNoSee);

			var cc = await guild.CreateTextChannelAsync("criticisms");
			await cc.ModifyAsync(x => x.CategoryId = category.Id);
			await cc.AddPermissionOverwriteAsync(vetmodrole, permsSee);
			await cc.AddPermissionOverwriteAsync(vetrole, permsSee);
			await cc.AddPermissionOverwriteAsync(guild.EveryoneRole, permsNoSee);

			var ac = await guild.CreateTextChannelAsync("afterthoughts");
			await ac.ModifyAsync(x => x.CategoryId = category.Id);
			await ac.AddPermissionOverwriteAsync(vetmodrole, permsSee);
			await ac.AddPermissionOverwriteAsync(vetrole, permsSee);
			await ac.AddPermissionOverwriteAsync(guild.EveryoneRole, permsNoSee);
			
			// update player roles
			int numsurvivors = 0;
			foreach (SocketGuildUser user in guild.Users) {
				var userRoles = user.Roles;

				// users should not have both of these
				if (userRoles.Contains(modrole))				// convert mod role to vetmod role
					await user.AddRoleAsync(vetmodrole);
				else if (userRoles.Contains(playerrole))		// convert player role to vet role
					await user.AddRoleAsync(vetrole);

				if (survivors)                                  // confirm if players actually survived or just didn't show
					if (userRoles.Contains(humanrole)) {        // convert humans into survivors
						guildAccount.gameLog.PlayerSurvivedMessage(Accounts.GetUser(user));
						await user.AddRoleAsync(survivorrole);
						numsurvivors++;
					}
			}

			await DeleteRoles(guild);

			guildAccount.gameLog.EndMessage(survivors, numsurvivors);
			guildAccount.gameLog.endTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
			guildAccount.gameData.active = false;
			Accounts.SaveAccounts();
		}
	}
}
