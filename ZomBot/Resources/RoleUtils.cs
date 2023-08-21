using Discord.WebSocket;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZomBot.Data;

namespace ZomBot.Resources {
	public static class RoleUtils {
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

			Program.Info($"Created new role for clan: {clanName}");
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
					Program.Info("Created new mod role.");
					updated = true;
				}
			}

			if (!endgame) { // mvz role
				var temprole = from r in roles where r.Id == guildAccount.roleIDs.mvz select r;
				var role = temprole.FirstOrDefault();

				if (role == null) {
					guildAccount.roleIDs.mvz = (await guild.CreateRoleAsync("MVZ", color: Color.DarkGreen, isHoisted: true, isMentionable: true)).Id;
					Program.Info("Created new mvz role.");
					updated = true;
				}
			}

			if (!endgame) { // zombie role
				var temprole = from r in roles where r.Id == guildAccount.roleIDs.zombie select r;
				var role = temprole.FirstOrDefault();

				if (role == null) {
					guildAccount.roleIDs.zombie = (await guild.CreateRoleAsync("Zombie", color: Color.Green, isHoisted: false, isMentionable: true)).Id;
					Program.Info("Created new zombie role.");
					updated = true;
				}
			}

			if (!endgame) { // cured role
				var temprole = from r in roles where r.Id == guildAccount.roleIDs.cured select r;
				var role = temprole.FirstOrDefault();

				if (role == null) {
					guildAccount.roleIDs.cured = (await guild.CreateRoleAsync("Cured", color: Color.Orange, isHoisted: false, isMentionable: true)).Id;
					Program.Info("Created new revived role.");
					updated = true;
				}
			}

			if (!endgame) { // human role
				var temprole = from r in roles where r.Id == guildAccount.roleIDs.human select r;
				var role = temprole.FirstOrDefault();

				if (role == null) {
					guildAccount.roleIDs.human = (await guild.CreateRoleAsync("Human", color: Color.LightOrange, isHoisted: false, isMentionable: true)).Id;
					Program.Info("Created new human role.");
					updated = true;
				}
			}

			{ // player role
				var temprole = from r in roles where r.Id == guildAccount.roleIDs.player select r;
				var role = temprole.FirstOrDefault();

				if (role == null) {
					guildAccount.roleIDs.player = (await guild.CreateRoleAsync("Player", color: Color.Default, isHoisted: false, isMentionable: true)).Id;
					Program.Info("Created new player role.");
					updated = true;
				}
			}

			if (updated)
				Accounts.SaveAccounts();
		}

		public static async Task DeleteRoles(SocketGuild guild) {
			var guildAccount = Accounts.GetGuild(guild);
			var roles = guild.Roles;

			{ // mod -> veteran mod role
				var temprole = from r in roles where r.Id == guildAccount.roleIDs.mod select r;
				var role = temprole.FirstOrDefault();

				if (role != null) {
					guildAccount.roleIDs.mod = 0;
					await guild.GetRole(role.Id).ModifyAsync(x => x.Name = "Veteran Mod");
					Program.Info("Updated mod role.");
				}
			}

			{ // mvz role
				var temprole = from r in roles where r.Id == guildAccount.roleIDs.mvz select r;
				var role = temprole.FirstOrDefault();

				if (role != null) {
					guildAccount.roleIDs.mvz = 0;
					await guild.GetRole(role.Id).DeleteAsync();
					Program.Info("Deleted mvz role.");
				}
			}

			{ // zombie role
				var temprole = from r in roles where r.Id == guildAccount.roleIDs.zombie select r;
				var role = temprole.FirstOrDefault();

				if (role != null) {
					guildAccount.roleIDs.zombie = 0;
					await guild.GetRole(role.Id).DeleteAsync();
					Program.Info("Deleted zombie role.");
				}
			}

			{ // human role
				var temprole = from r in roles where r.Id == guildAccount.roleIDs.human select r;
				var role = temprole.FirstOrDefault();

				if (role != null) {
					guildAccount.roleIDs.human = 0;
					await guild.GetRole(role.Id).DeleteAsync();
					Program.Info("Deleted human role.");
				}
			}

			{ // cured role
				var temprole = from r in roles where r.Id == guildAccount.roleIDs.cured select r;
				var role = temprole.FirstOrDefault();

				if (role != null) {
					guildAccount.roleIDs.cured = 0;
					await guild.GetRole(role.Id).DeleteAsync();
					Program.Info("Deleted cured role.");
				}
			}

			{ // player -> veteran role
				var temprole = from r in roles where r.Id == guildAccount.roleIDs.player select r;
				var role = temprole.FirstOrDefault();

				if (role != null) {
					guildAccount.roleIDs.player = 0;
					await guild.GetRole(role.Id).ModifyAsync(x => x.Name = "Veteran");
					Program.Info("Updated player role.");
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
						Program.Info($"Removed {clanName.Trim()} role from {user.Username}.");
						await userButInGuild.RemoveRoleAsync(role);
					}
				} else {
					if (c.clanName.Trim().ToLower() == clanName.Trim().ToLower()) {
						Program.Info($"Added {clanName.Trim()} role to {user.Username}.");
						await userButInGuild.AddRoleAsync(role);
					}
				}
			}

			if (toRemove.Count() > 0) {
				foreach (Clan c in toRemove) {
					Program.Info($"Deleted clan: {c.clanName}.");
					guildData.clanList.Remove(c);
				}

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

				if (userRoles.Contains(role)) {
					await userButInGuild.RemoveRoleAsync(role);
					Program.Info($"Removed {c.clanName} role from {user.Username}.");
				}

				if (role.Members.Count() == 0) {
					Program.Info($"Deleted role: {role.Name}.");
					await role.DeleteAsync();
					toRemove.Add(c);
				}
			}

			if (toRemove.Count() > 0) {
				foreach (Clan c in toRemove) {
					Program.Info($"Deleted clan: {c.clanName}.");
					guildData.clanList.Remove(c);
				}

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
				if (role.Id == guildData.roleIDs.mod) {
					await userButInGuild.RemoveRoleAsync(guildData.roleIDs.mod);
					Program.Info($"Removed {role.Name} role from {userButInGuild.Username}.");
				} else if (role.Id == guildData.roleIDs.mvz) {
					await userButInGuild.RemoveRoleAsync(guildData.roleIDs.mvz);
					Program.Info($"Removed {role.Name} role from {userButInGuild.Username}.");
				} else if (role.Id == guildData.roleIDs.zombie) {
					await userButInGuild.RemoveRoleAsync(guildData.roleIDs.zombie);
					Program.Info($"Removed {role.Name} role from {userButInGuild.Username}.");
				} else if (role.Id == guildData.roleIDs.human)
					addHuman = false;
				else if (role.Id == guildData.roleIDs.player)
					addPlayer = false;
				else if (role.Id == guildData.roleIDs.cured)
					addCured = false;
			}

			if (addHuman) {
				await userButInGuild.AddRoleAsync(guildData.roleIDs.human);
				Program.Info($"Added Human role to {userButInGuild.Username}.");
			}

			if (addPlayer) {
				await userButInGuild.AddRoleAsync(guildData.roleIDs.player);
				Program.Info($"Added Player role to {userButInGuild.Username}.");
			}

			if (addCured) {
				await userButInGuild.AddRoleAsync(guildData.roleIDs.cured);
				Program.Info($"Added Cured role to {userButInGuild.Username}.");
			}
		}

		public static async Task JoinZombieTeam(IUser user, SocketGuild guild, bool isMVZ = false) {
			await CreateRoles(guild);
			var guildData = Accounts.GetGuild(guild.Id);
			var userButInGuild = guild.GetUser(user.Id);
			bool addZombie = true, addPlayer = true, addMVZ = true;

			foreach (SocketRole role in userButInGuild.Roles) {
				if (role.Id == guildData.roleIDs.mod) {
					await userButInGuild.RemoveRoleAsync(guildData.roleIDs.mod);
					Program.Info($"Removed {role.Name} role from {userButInGuild.Username}.");
				} else if (role.Id == guildData.roleIDs.human) {
					await userButInGuild.RemoveRoleAsync(guildData.roleIDs.human);
					Program.Info($"Removed {role.Name} role from {userButInGuild.Username}.");
				} else if (role.Id == guildData.roleIDs.cured) {
					await userButInGuild.RemoveRoleAsync(guildData.roleIDs.cured);
					Program.Info($"Removed {role.Name} role from {userButInGuild.Username}.");
				} else if (role.Id == guildData.roleIDs.zombie)
					addZombie = false;
				else if (role.Id == guildData.roleIDs.player)
					addPlayer = false;
				else if (role.Id == guildData.roleIDs.mvz) {
					if (isMVZ)
						addMVZ = false;
					else {
						await userButInGuild.RemoveRoleAsync(guildData.roleIDs.mvz);
						Program.Info($"Removed {role.Name} role from {userButInGuild.Username}.");
					}
				}
			}

			if (addZombie) {
				await userButInGuild.AddRoleAsync(guildData.roleIDs.zombie);
				Program.Info($"Added Zombie role to {userButInGuild.Username}.");
			}

			if (addPlayer) {
				await userButInGuild.AddRoleAsync(guildData.roleIDs.player);
				Program.Info($"Added Player role to {userButInGuild.Username}.");
			}

			if (addMVZ && isMVZ) {
				await userButInGuild.AddRoleAsync(guildData.roleIDs.mvz);
				Program.Info($"Added MVZ role to {userButInGuild.Username}.");
			}
		}

		public static async Task JoinModTeam(IUser user, SocketGuild guild) {
			await CreateRoles(guild);
			var guildData = Accounts.GetGuild(guild.Id);
			var userButInGuild = guild.GetUser(user.Id);
			bool addMod = true;

			foreach (SocketRole role in userButInGuild.Roles) {
				if (role.Id == guildData.roleIDs.mod)
					addMod = false;
				else if (role.Id == guildData.roleIDs.mvz) {
					await userButInGuild.RemoveRoleAsync(guildData.roleIDs.mvz);
					Program.Info($"Removed {role.Name} role from {userButInGuild.Username}.");
				} else if (role.Id == guildData.roleIDs.zombie) {
					await userButInGuild.RemoveRoleAsync(guildData.roleIDs.zombie);
					Program.Info($"Removed {role.Name} role from {userButInGuild.Username}.");
				} else if (role.Id == guildData.roleIDs.human) {
					await userButInGuild.RemoveRoleAsync(guildData.roleIDs.human);
					Program.Info($"Removed {role.Name} role from {userButInGuild.Username}.");
				} else if (role.Id == guildData.roleIDs.cured) {
					await userButInGuild.RemoveRoleAsync(guildData.roleIDs.cured);
					Program.Info($"Removed {role.Name} role from {userButInGuild.Username}.");
				} else if (role.Id == guildData.roleIDs.player) {
					await userButInGuild.RemoveRoleAsync(guildData.roleIDs.player);
					Program.Info($"Removed {role.Name} role from {userButInGuild.Username}.");
				}
			}

			if (addMod) {
				await userButInGuild.AddRoleAsync(guildData.roleIDs.mod);
				Program.Info($"Added Mod role to {userButInGuild.Username}.");
			}
		}

		public static async Task LeaveTeams(IUser user, SocketGuild guild) {
			await CreateRoles(guild);
			var guildData = Accounts.GetGuild(guild.Id);
			var userButInGuild = guild.GetUser(user.Id);

			foreach (SocketRole role in userButInGuild.Roles) {
				if (role.Id == guildData.roleIDs.mod) {
					await userButInGuild.RemoveRoleAsync(guildData.roleIDs.mod);
					Program.Info($"Removed {role.Name} role from {userButInGuild.Username}.");
				} else if (role.Id == guildData.roleIDs.mvz) {
					await userButInGuild.RemoveRoleAsync(guildData.roleIDs.mvz);
					Program.Info($"Removed {role.Name} role from {userButInGuild.Username}.");
				} else if (role.Id == guildData.roleIDs.zombie) {
					await userButInGuild.RemoveRoleAsync(guildData.roleIDs.zombie);
					Program.Info($"Removed {role.Name} role from {userButInGuild.Username}.");
				} else if (role.Id == guildData.roleIDs.human) {
					await userButInGuild.RemoveRoleAsync(guildData.roleIDs.human);
					Program.Info($"Removed {role.Name} role from {userButInGuild.Username}.");
				} else if (role.Id == guildData.roleIDs.cured) {
					await userButInGuild.RemoveRoleAsync(guildData.roleIDs.cured);
					Program.Info($"Removed {role.Name} role from {userButInGuild.Username}.");
				} else if (role.Id == guildData.roleIDs.player) {
					await userButInGuild.RemoveRoleAsync(guildData.roleIDs.player);
					Program.Info($"Removed {role.Name} role from {userButInGuild.Username}.");
				}
			}
		}
	}
}
