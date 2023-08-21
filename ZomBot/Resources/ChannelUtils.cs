using Discord.WebSocket;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZomBot.Data;

namespace ZomBot.Resources {
	public static class ChannelUtils {
		public static async void UpdateChannel(ulong channelID, SocketGuild guild) {
			await RoleUtils.CreateRoles(guild);
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
					Program.Info($"Updated {channel.Name} as {guildAccount.channels.GetChannelType(channelID)}.");
					return;
				case ChannelDesignation.LOG:
					await channel.AddPermissionOverwriteAsync(guild.EveryoneRole, permsNoSee);
					Program.Info($"Updated {channel.Name} as {guildAccount.channels.GetChannelType(channelID)}.");
					return;
				case ChannelDesignation.TAG:
					await channel.AddPermissionOverwriteAsync(guild.GetRole(guildAccount.roleIDs.mod), permsSeeNoSpeak);
					await channel.AddPermissionOverwriteAsync(guild.GetRole(guildAccount.roleIDs.human), permsSeeNoSpeak);
					await channel.AddPermissionOverwriteAsync(guild.GetRole(guildAccount.roleIDs.zombie), permsSeeNoSpeak);
					await channel.AddPermissionOverwriteAsync(guild.EveryoneRole, permsNoSee);
					Program.Info($"Updated {channel.Name} as {guildAccount.channels.GetChannelType(channelID)}.");
					return;
				case ChannelDesignation.HUMAN:
					await channel.AddPermissionOverwriteAsync(guild.GetRole(guildAccount.roleIDs.mod), permsManage);
					await channel.AddPermissionOverwriteAsync(guild.GetRole(guildAccount.roleIDs.human), permsSee);
					await channel.AddPermissionOverwriteAsync(guild.EveryoneRole, permsNoSee);
					Program.Info($"Updated {channel.Name} as {guildAccount.channels.GetChannelType(channelID)}.");
					return;
				case ChannelDesignation.HUMANANNOUNCEMENT:
					await channel.AddPermissionOverwriteAsync(guild.GetRole(guildAccount.roleIDs.mod), permsManage);
					await channel.AddPermissionOverwriteAsync(guild.GetRole(guildAccount.roleIDs.human), permsSeeNoSpeak);
					await channel.AddPermissionOverwriteAsync(guild.EveryoneRole, permsNoSee);
					Program.Info($"Updated {channel.Name} as {guildAccount.channels.GetChannelType(channelID)}.");
					return;
				case ChannelDesignation.ZOMBIE:
					await channel.AddPermissionOverwriteAsync(guild.GetRole(guildAccount.roleIDs.mod), permsManage);
					await channel.AddPermissionOverwriteAsync(guild.GetRole(guildAccount.roleIDs.zombie), permsSee);
					await channel.AddPermissionOverwriteAsync(guild.EveryoneRole, permsNoSee);
					Program.Info($"Updated {channel.Name} as {guildAccount.channels.GetChannelType(channelID)}.");
					return;
				case ChannelDesignation.ZOMBIEANNOUNCEMENT:
					await channel.AddPermissionOverwriteAsync(guild.GetRole(guildAccount.roleIDs.mod), permsManage);
					await channel.AddPermissionOverwriteAsync(guild.GetRole(guildAccount.roleIDs.zombie), permsSeeNoSpeak);
					await channel.AddPermissionOverwriteAsync(guild.EveryoneRole, permsNoSee);
					Program.Info($"Updated {channel.Name} as {guildAccount.channels.GetChannelType(channelID)}.");
					return;
				case ChannelDesignation.SHAREDANNOUNCEMENT:
					await channel.AddPermissionOverwriteAsync(guild.GetRole(guildAccount.roleIDs.mod), permsManage);
					await channel.AddPermissionOverwriteAsync(guild.GetRole(guildAccount.roleIDs.human), permsSeeNoSpeak);
					await channel.AddPermissionOverwriteAsync(guild.GetRole(guildAccount.roleIDs.zombie), permsSeeNoSpeak);
					await channel.AddPermissionOverwriteAsync(guild.EveryoneRole, permsNoSee);
					Program.Info($"Updated {channel.Name} as {guildAccount.channels.GetChannelType(channelID)}.");
					return;
				default: // null
					break;
			}
		}

		public static async Task EndGame(SocketGuild guild) {
			await RoleUtils.CreateRoles(guild, true);
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
							Program.Info($"Updated {textChannel.Name} as {channel.type}.");
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
							Program.Info($"Updated {textChannel.Name} as {channel.type}.");
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
			Program.Info("Created postgame category.");

			var pac = await guild.CreateTextChannelAsync("announcements");
			await pac.ModifyAsync(x => x.CategoryId = category.Id);
			await pac.AddPermissionOverwriteAsync(modRole, permsSee);
			await pac.AddPermissionOverwriteAsync(playerRole, permsSeeNoSpeak);
			await pac.AddPermissionOverwriteAsync(everyoneRole, permsNoSee);
			Program.Info("Created postgame announcements channel.");

			var cc = await guild.CreateTextChannelAsync("criticisms");
			await cc.ModifyAsync(x => x.CategoryId = category.Id);
			await cc.AddPermissionOverwriteAsync(modRole, permsSee);
			await cc.AddPermissionOverwriteAsync(playerRole, permsSee);
			await cc.AddPermissionOverwriteAsync(everyoneRole, permsNoSee);
			Program.Info("Created postgame criticisms channel.");

			var ac = await guild.CreateTextChannelAsync("afterthoughts");
			await ac.ModifyAsync(x => x.CategoryId = category.Id);
			await ac.AddPermissionOverwriteAsync(modRole, permsSee);
			await ac.AddPermissionOverwriteAsync(playerRole, permsSee);
			await ac.AddPermissionOverwriteAsync(everyoneRole, permsNoSee);
			Program.Info("Created postgame afterthoughts channel.");

			await pac.SendMessageAsync($"{playerRole.Mention} Please use {cc.Mention} and {ac.Mention} to leave your feedback of your experiences during this semester's HvZ!");
			await RoleUtils.DeleteRoles(guild);
			Accounts.SaveAccounts();
		}
	}
}
