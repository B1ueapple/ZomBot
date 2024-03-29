﻿using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Threading.Tasks;
using ZomBot.Data;
using ZomBot.Resources;

namespace ZomBot.Commands {
    public class Channels : InteractionModuleBase {
        public enum RegisterParameter {
            Register,
            Unregister
        }

        [SlashCommand("channels", "Register or unregister the current channel.")]
        [RequireContext(ContextType.Guild)]
        [DefaultMemberPermissions(GuildPermission.ManageChannels), RequireBotPermission(GuildPermission.ManageChannels)]
        public async Task RegisterChannelCommand([Summary("Mode", "Choose to register or unregister this channel.")] RegisterParameter register, [Summary("Type", "What type of channel to register as.")] ChannelDesignation? type = null) {
            if (Context.Guild is SocketGuild guild) {
                var guildAccount = Accounts.GetGuild(guild);
                ulong channelID = Context.Channel.Id;

                if (register == RegisterParameter.Unregister) {
                    if (guildAccount.channels.Remove(channelID)) {
                        Accounts.SaveAccounts();
                        await RespondAsync($":white_check_mark: #{Context.Channel.Name} is no longer a registed channel.", ephemeral: true);
                    } else
                        await RespondAsync($":x: #{Context.Channel.Name} is not a registered channel.", ephemeral: true);

                    return;
				}

				if (type == null) {
					await RespondAsync(":x: Type required to register channel.", ephemeral: true);
					return;
                }

				if (type == ChannelDesignation.LOG) {
					await RespondAsync(":x: LOG is reserved for internal use.", ephemeral: true);
                    return;
				}

                ChannelDesignation typeNotNull = (ChannelDesignation)type;
				if (IsUnique(typeNotNull)) {
					if (guildAccount.channels.AddUnique(channelID, typeNotNull)) {
						Accounts.SaveAccounts();
						ChannelUtils.UpdateChannel(channelID, guild);
						await RespondAsync($":white_check_mark: Registered #{Context.Channel.Name} as the {typeNotNull} channel.", ephemeral: true);
                        Program.Info($"Registered #{Context.Channel.Name} as the {typeNotNull} channel.");
                    } else
						await RespondAsync($":question: #{Context.Channel.Name} is already registered as the {typeNotNull} channel.", ephemeral: true);
				} else {
					if (guildAccount.channels.Add(channelID, typeNotNull)) {
						Accounts.SaveAccounts();
						ChannelUtils.UpdateChannel(channelID, guild);
						await RespondAsync($":white_check_mark: Registered #{Context.Channel.Name} as a {typeNotNull} channel.", ephemeral: true);
						Program.Info($"Registered #{Context.Channel.Name} as the {typeNotNull} channel.");
					} else
						await RespondAsync($":question: #{Context.Channel.Name}  is already registered as a {typeNotNull} channel.", ephemeral: true);
				}
            } else
                await RespondAsync(":x: This command can't be used here.", ephemeral: true);
		}

        private bool IsUnique(ChannelDesignation type) {
            switch (type) {
                case ChannelDesignation.MODIMPORTANT:
                case ChannelDesignation.HUMANANNOUNCEMENT:
                case ChannelDesignation.ZOMBIEANNOUNCEMENT:
                case ChannelDesignation.SHAREDANNOUNCEMENT:
                case ChannelDesignation.TAG:
                    return true;
                case ChannelDesignation.MOD:
                case ChannelDesignation.HUMAN:
                case ChannelDesignation.ZOMBIE:
                    return false;
                default:
                    return true;
            }
        }
    }
}
