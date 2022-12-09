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

        public static async void CreateRoles(SocketGuild guild) {
            var guildAccount = Accounts.GetGuild(guild);
            bool updated = false;
            
            if (guild.GetRole(guildAccount.roleIDs.mod) == null) {
                var modRole = await guild.CreateRoleAsync("Mod", color: Color.Blue, isHoisted: true, isMentionable: true);

                guildAccount.roleIDs.mod = modRole.Id;
                Console.WriteLine("Created new mod role.");
                updated = true;
            }

            if (guild.GetRole(guildAccount.roleIDs.mvz) == null) {
                guildAccount.roleIDs.mvz = (await guild.CreateRoleAsync("MVZ", color: Color.DarkGreen, isHoisted: true, isMentionable: true)).Id;
                Console.WriteLine("Created new mvz role.");
                updated = true;
            }

            if (guild.GetRole(guildAccount.roleIDs.zombie) == null) {
                guildAccount.roleIDs.zombie = (await guild.CreateRoleAsync("Zombie", color: Color.Green, isHoisted: false, isMentionable: true)).Id;
                Console.WriteLine("Created new zombie role.");
                updated = true;
            }

            if (guild.GetRole(guildAccount.roleIDs.human) == null) {
                guildAccount.roleIDs.human = (await guild.CreateRoleAsync("Human", color: Color.LightOrange, isHoisted: false, isMentionable: true)).Id;
                Console.WriteLine("Created new human role.");
                updated = true;
            }

            if (guild.GetRole(guildAccount.roleIDs.player) == null) {
                guildAccount.roleIDs.player = (await guild.CreateRoleAsync("Player", color: Color.Default, isHoisted: false, isMentionable: true)).Id;
                Console.WriteLine("Created new player role.");
                updated = true;
            }

            if (updated)
                Accounts.SaveAccounts();
        }

        public static async Task JoinClan(IUser user, SocketGuild guild, string clanName) {
            await CreateClanRole(guild, clanName);
            var guildData = Accounts.GetGuild(guild.Id);
            var userButInGuild = guild.GetUser(user.Id);
            var roles = userButInGuild.Roles;
            
            foreach (Clan c in guildData.clanList) {
                var role = guild.GetRole(c.roleID);

                if (roles.Contains(role)) {
                    if (c.clanName.ToLower() != clanName.ToLower()) {
                        await userButInGuild.RemoveRoleAsync(role);
                    }
                } else {
                    if (c.clanName.ToLower() == clanName.ToLower()) {
                        await userButInGuild.AddRoleAsync(role);
                    }
                }
            }
        }

        public static async Task LeaveClan(IUser user, SocketGuild guild) {
            var guildData = Accounts.GetGuild(guild.Id);
            var userButInGuild = guild.GetUser(user.Id);
            var roles = userButInGuild.Roles;

            foreach (Clan c in guildData.clanList) {
                var role = guild.GetRole(c.roleID);

                if (roles.Contains(role))
                    await userButInGuild.RemoveRoleAsync(role);
            }
        }

        public static async Task JoinHumanTeam(IUser user, SocketGuild guild) {
            CreateRoles(guild);
            var guildData = Accounts.GetGuild(guild.Id);
            var userButInGuild = guild.GetUser(user.Id);
            bool addHuman = true, addPlayer = true;

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
            }

            if (addHuman)
                await userButInGuild.AddRoleAsync(guildData.roleIDs.human);

            if (addPlayer)
                await userButInGuild.AddRoleAsync(guildData.roleIDs.player);
        }

        public static async Task JoinZombieTeam(IUser user, SocketGuild guild, bool isMVZ = false) {
            CreateRoles(guild);
            var guildData = Accounts.GetGuild(guild.Id);
            var userButInGuild = guild.GetUser(user.Id);
            bool addZombie = true, addPlayer = true, addMVZ = true;

            foreach (SocketRole role in userButInGuild.Roles) {
                if (role.Id == guildData.roleIDs.mod)
                    await userButInGuild.RemoveRoleAsync(guildData.roleIDs.mod);
                else if (role.Id == guildData.roleIDs.human)
                    await userButInGuild.RemoveRoleAsync(guildData.roleIDs.human);
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
            CreateRoles(guild);
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
                else if (role.Id == guildData.roleIDs.player)
                    await userButInGuild.RemoveRoleAsync(guildData.roleIDs.player);
            }

            if (addMod)
                await userButInGuild.AddRoleAsync(guildData.roleIDs.mod);
        }

        public static async Task LeaveTeams(IUser user, SocketGuild guild) {
            CreateRoles(guild);
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
            CreateRoles(guild);
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
    }
}
