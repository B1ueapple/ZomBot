using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Threading.Tasks;
using ZomBot.Data;
using ZomBot.Resources;

namespace ZomBot.Commands {
    public class Channels : InteractionModuleBase {
        [SlashCommand("mic", "Set the current channel as a mod important channel.")]
        [RequireContext(ContextType.Guild)]
        [DefaultMemberPermissions(GuildPermission.ManageChannels), RequireBotPermission(GuildPermission.ManageChannels)]
        public async Task SetModImportantChannelCommand([Summary("Remove", "Set to true to remove.")] bool reset = false) {
            if (Context.Guild is SocketGuild guild) {
                var guildAccount = Accounts.GetGuild(guild);
                ulong channelID = Context.Channel.Id;

                if (reset) {
                    if (guildAccount.channels.Remove(channelID)) {
                        Accounts.SaveAccounts();
                        await RespondAsync($":thumbsup: #{Context.Channel.Name} is no longer a registed channel :thumbsup:", ephemeral: true);
                    } else
                        await RespondAsync($":x: #{Context.Channel.Name} is not a registered channel :x:", ephemeral: true);

                    return;
                }

                if (guildAccount.channels.AddUnique(channelID, ChannelDesignation.MODIMPORTANT)) {
                    Accounts.SaveAccounts();
                    RoleHandler.UpdateChannel(channelID, guild);
                    await RespondAsync($":thumbsup: Set #{Context.Channel.Name} as the mod important channel :thumbsup:", ephemeral: true);
                } else
                    await RespondAsync($":question: #{Context.Channel.Name} is already the mod important channel :question:", ephemeral: true);
            } else
                await RespondAsync(":x: This command can't be used here :x:", ephemeral: true);
        }

        [SlashCommand("zc", "Set the current channel as a zombie text channel.")]
        [RequireContext(ContextType.Guild)]
        [DefaultMemberPermissions(GuildPermission.ManageChannels), RequireBotPermission(GuildPermission.ManageChannels)]
        public async Task SetZombieChannelCommand([Summary("Remove", "Set to true to remove.")] bool reset = false) {
            if (Context.Guild is SocketGuild guild) {
                var guildAccount = Accounts.GetGuild(guild);
                ulong channelID = Context.Channel.Id;

                if (reset) {
					if (guildAccount.channels.Remove(channelID)) {
						Accounts.SaveAccounts();
						await RespondAsync($":thumbsup: #{Context.Channel.Name} is no longer a registed channel :thumbsup:", ephemeral: true);
					} else
						await RespondAsync($":x: #{Context.Channel.Name} is not a registered channel :x:", ephemeral: true);

					return;
				}

                if (guildAccount.channels.Add(channelID, ChannelDesignation.ZOMBIE)) {
                    Accounts.SaveAccounts();
                    RoleHandler.UpdateChannel(channelID, guild);
                    await RespondAsync($":thumbsup: Added #{Context.Channel.Name} to zombie channels :thumbsup:", ephemeral: true);
                } else
                    await RespondAsync($":question: #{Context.Channel.Name} is already in zombie channels :question:", ephemeral: true);
            } else
                await RespondAsync(":x: This command can't be used here :x:", ephemeral: true);
        }

        [SlashCommand("hc", "Set the current channel as a human text channel.")]
        [RequireContext(ContextType.Guild)]
        [DefaultMemberPermissions(GuildPermission.ManageChannels), RequireBotPermission(GuildPermission.ManageChannels)]
        public async Task SetHumanChannelCommand([Summary("Remove", "Set to true to remove.")] bool reset = false) {
            if (Context.Guild is SocketGuild guild) {
                var guildAccount = Accounts.GetGuild(guild);
                ulong channelID = Context.Channel.Id;

                if (reset) {
					if (guildAccount.channels.Remove(channelID)) {
						Accounts.SaveAccounts();
						await RespondAsync($":thumbsup: #{Context.Channel.Name} is no longer a registed channel :thumbsup:", ephemeral: true);
					} else
						await RespondAsync($":x: #{Context.Channel.Name} is not a registered channel :x:", ephemeral: true);

					return;
				}

                if (guildAccount.channels.Add(channelID, ChannelDesignation.HUMAN)) {
                    Accounts.SaveAccounts();
                    RoleHandler.UpdateChannel(channelID, guild);
                    await RespondAsync($":thumbsup: Added #{Context.Channel.Name} to human channels :thumbsup:", ephemeral: true);
                } else
                    await RespondAsync($":question: #{Context.Channel.Name} is already in human channels :question:", ephemeral: true);
            } else
                await RespondAsync(":x: This command can't be used here :x:", ephemeral: true);
        }
        
        [SlashCommand("mc", "Set the current channel as a mod text channel.")]
        [RequireContext(ContextType.Guild)]
        [DefaultMemberPermissions(GuildPermission.ManageChannels), RequireBotPermission(GuildPermission.ManageChannels)]
        public async Task SetModChannelCommand([Summary("Remove", "Set to true to remove.")] bool reset = false) {
            if (Context.Guild is SocketGuild guild) {
                var guildAccount = Accounts.GetGuild(guild);
                ulong channelID = Context.Channel.Id;

                if (reset) {
					if (guildAccount.channels.Remove(channelID)) {
						Accounts.SaveAccounts();
						await RespondAsync($":thumbsup: #{Context.Channel.Name} is no longer a registed channel :thumbsup:", ephemeral: true);
					} else
						await RespondAsync($":x: #{Context.Channel.Name} is not a registered channel :x:", ephemeral: true);

					return;
				}

                if (guildAccount.channels.Add(channelID, ChannelDesignation.MOD)) {
                    Accounts.SaveAccounts();
                    RoleHandler.UpdateChannel(channelID, guild);
                    await RespondAsync($":thumbsup: Added #{Context.Channel.Name} to mod channels :thumbsup:", ephemeral: true);
                } else
                    await RespondAsync($":question: #{Context.Channel.Name} is already in mod channels :question:", ephemeral: true);
            } else
                await RespondAsync(":x: This command can't be used here :x:", ephemeral: true);
        }
        
        [SlashCommand("gac", "Set the current channel as the general announcement channel.")]
        [RequireContext(ContextType.Guild)]
        [DefaultMemberPermissions(GuildPermission.ManageChannels), RequireBotPermission(GuildPermission.ManageChannels)]
        public async Task SetGeneralAnnouncementChannelCommand([Summary("Remove", "Set to true to remove.")] bool reset = false) {
            if (Context.Guild is SocketGuild guild) {
                var guildAccount = Accounts.GetGuild(guild);
                ulong channelID = Context.Channel.Id;

                if (reset) {
					if (guildAccount.channels.Remove(channelID)) {
						Accounts.SaveAccounts();
						await RespondAsync($":thumbsup: #{Context.Channel.Name} is no longer a registed channel :thumbsup:", ephemeral: true);
					} else
						await RespondAsync($":x: #{Context.Channel.Name} is not a registered channel :x:", ephemeral: true);

					return;
				}

                if (guildAccount.channels.AddUnique(channelID, ChannelDesignation.SHAREDANNOUNCEMENT)) {
                    Accounts.SaveAccounts();
                    RoleHandler.UpdateChannel(channelID, guild);
                    await RespondAsync($":thumbsup: Set #{Context.Channel.Name} as the general announcement channel :thumbsup:", ephemeral: true);
                    return;
                } else
                    await RespondAsync($":question: #{Context.Channel.Name} is already the general announcement channel :question:", ephemeral: true);
            } else
                await RespondAsync(":x: This command can't be used here :x:", ephemeral: true);
        }
        
        [SlashCommand("zac", "Set the current channel as the zombie announcement channel.")]
        [RequireContext(ContextType.Guild)]
        [DefaultMemberPermissions(GuildPermission.ManageChannels), RequireBotPermission(GuildPermission.ManageChannels)]
        public async Task SetZombieAnnouncementChannelCommand([Summary("Remove", "Set to true to remove.")] bool reset = false) {
            if (Context.Guild is SocketGuild guild) {
                var guildAccount = Accounts.GetGuild(guild);
                ulong channelID = Context.Channel.Id;

                if (reset) {
					if (guildAccount.channels.Remove(channelID)) {
						Accounts.SaveAccounts();
						await RespondAsync($":thumbsup: #{Context.Channel.Name} is no longer a registed channel :thumbsup:", ephemeral: true);
					} else
						await RespondAsync($":x: #{Context.Channel.Name} is not a registered channel :x:", ephemeral: true);

					return;
				}

                if (guildAccount.channels.AddUnique(channelID, ChannelDesignation.ZOMBIEANNOUNCEMENT)) {
                    Accounts.SaveAccounts();
                    RoleHandler.UpdateChannel(channelID, guild);
                    await RespondAsync($":thumbsup: Set #{Context.Channel.Name} as the zombie announcement channel :thumbsup:", ephemeral: true);
                    return;
                } else
                    await RespondAsync($":question: #{Context.Channel.Name} is already the zombie announcement channel :question:", ephemeral: true);
            } else
                await RespondAsync(":x: This command can't be used here :x:", ephemeral: true);
        }
        
        [SlashCommand("hac", "Set the current channel as the human announcement channel.")]
        [RequireContext(ContextType.Guild)]
        [DefaultMemberPermissions(GuildPermission.ManageChannels), RequireBotPermission(GuildPermission.ManageChannels)]
        public async Task SetHumanAnnouncementChannelCommand([Summary("Remove", "Set to true to remove.")] bool reset = false) {
            if (Context.Guild is SocketGuild guild) {
                var guildAccount = Accounts.GetGuild(guild);
                ulong channelID = Context.Channel.Id;

                if (reset) {
					if (guildAccount.channels.Remove(channelID)) {
						Accounts.SaveAccounts();
						await RespondAsync($":thumbsup: #{Context.Channel.Name} is no longer a registed channel :thumbsup:", ephemeral: true);
					} else
						await RespondAsync($":x: #{Context.Channel.Name} is not a registered channel :x:", ephemeral: true);

					return;
				}

                if (guildAccount.channels.AddUnique(channelID, ChannelDesignation.HUMANANNOUNCEMENT)) {
                    Accounts.SaveAccounts();
                    RoleHandler.UpdateChannel(channelID, guild);
                    await RespondAsync($":thumbsup: Set #{Context.Channel.Name} as the human announcement channel :thumbsup:", ephemeral: true);
                    return;
                } else
                    await RespondAsync($":question: #{Context.Channel.Name} is already the human announcement channel :question:", ephemeral: true);
            } else
                await RespondAsync(":x: This command can't be used here :x:", ephemeral: true);
        }
        
        [SlashCommand("tc", "Set the current channel as the tag channel.")]
        [RequireContext(ContextType.Guild)]
        [DefaultMemberPermissions(GuildPermission.ManageChannels), RequireBotPermission(GuildPermission.ManageChannels)]
        public async Task SetTagChannelCommand([Summary("Remove", "Set to true to remove.")] bool reset = false) {
            if (Context.Guild is SocketGuild guild) {
                var guildAccount = Accounts.GetGuild(guild);
                ulong channelID = Context.Channel.Id;

                if (reset) {
					if (guildAccount.channels.Remove(channelID)) {
						Accounts.SaveAccounts();
						await RespondAsync($":thumbsup: #{Context.Channel.Name} is no longer a registed channel :thumbsup:", ephemeral: true);
					} else
						await RespondAsync($":x: #{Context.Channel.Name} is not a registered channel :x:", ephemeral: true);

					return;
				}

                if (guildAccount.channels.AddUnique(channelID, ChannelDesignation.TAG)) {
                    Accounts.SaveAccounts();
                    RoleHandler.UpdateChannel(channelID, guild);
                    await RespondAsync($":thumbsup: Set #{Context.Channel.Name} as the tag channel :thumbsup:", ephemeral: true);
                    return;
                } else
                    await RespondAsync($":question: #{Context.Channel.Name} is already the tag channel :question:", ephemeral: true);
            } else
                await RespondAsync(":x: This command can't be used here :x:", ephemeral: true);
        }
    }
}
