using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Threading.Tasks;
using ZomBot.Data;
using ZomBot.Resources;

namespace ZomBot.Commands {
    public class Channels : InteractionModuleBase {
        [SlashCommand("zc", "Set the current channel as a zombie text channel.")]
        [RequireContext(ContextType.Guild)]
        [DefaultMemberPermissions(GuildPermission.ManageChannels), RequireBotPermission(GuildPermission.ManageChannels)]
        public async Task SetZombieChannelCommand([Summary("Remove", "Set to true to remove.")] bool reset = false) {
            if (Context.Guild is SocketGuild guild) {
                var guildAccount = Accounts.GetGuild(guild);
                ulong channelID = Context.Channel.Id;

                if (reset) {
                    if (guildAccount.channels.zombieChannels.Remove(channelID)) {
                        Accounts.SaveAccounts();
                        await RespondAsync($":thumbsup: Removed #{Context.Channel.Name} from zombie channels :thumbsup:", ephemeral: true);
                    } else
                        await RespondAsync($":x: #{Context.Channel.Name} is not in zombie channels :x:", ephemeral: true);

                    return;
                }

                guildAccount.channels.modChannels.Remove(channelID);
                guildAccount.channels.humanChannels.Remove(channelID);

                if (guildAccount.channels.generalAnnouncementChannel == channelID)
                    guildAccount.channels.generalAnnouncementChannel = 0;
                else if (guildAccount.channels.humanAnnouncementChannel == channelID)
                    guildAccount.channels.humanAnnouncementChannel = 0;
                else if (guildAccount.channels.zombieAnnouncementChannel == channelID)
                    guildAccount.channels.zombieAnnouncementChannel = 0;

                if (!guildAccount.channels.zombieChannels.Contains(channelID)) {
                    guildAccount.channels.zombieChannels.Add(channelID);
                    Accounts.SaveAccounts();
                    RoleHandler.UpdateChannel(channelID, guild);
                    await RespondAsync($":thumbsup: Added #{Context.Channel.Name} to zombie channels :thumbsup:", ephemeral: true);
                    return;
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
                    if (guildAccount.channels.humanChannels.Remove(channelID)) {
                        Accounts.SaveAccounts();
                        await RespondAsync($":thumbsup: Removed #{Context.Channel.Name} from human channels :thumbsup:", ephemeral: true);
                    } else
                        await RespondAsync($":x: #{Context.Channel.Name} is not in human channels :x:", ephemeral: true);

                    return;
                }

                guildAccount.channels.modChannels.Remove(channelID);
                guildAccount.channels.zombieChannels.Remove(channelID);

                if (guildAccount.channels.generalAnnouncementChannel == channelID)
                    guildAccount.channels.generalAnnouncementChannel = 0;
                else if (guildAccount.channels.humanAnnouncementChannel == channelID)
                    guildAccount.channels.humanAnnouncementChannel = 0;
                else if (guildAccount.channels.zombieAnnouncementChannel == channelID)
                    guildAccount.channels.zombieAnnouncementChannel = 0;

                if (!guildAccount.channels.humanChannels.Contains(channelID)) {
                    guildAccount.channels.humanChannels.Add(channelID);
                    Accounts.SaveAccounts();
                    RoleHandler.UpdateChannel(channelID, guild);
                    await RespondAsync($":thumbsup: Added #{Context.Channel.Name} to human channels :thumbsup:", ephemeral: true);
                    return;
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
                    if (guildAccount.channels.modChannels.Remove(channelID)) {
                        Accounts.SaveAccounts();
                        await RespondAsync($":thumbsup: Removed #{Context.Channel.Name} from mod channels :thumbsup:", ephemeral: true);
                    } else
                        await RespondAsync($":x: #{Context.Channel.Name} is not in mod channels :x:", ephemeral: true);

                    return;
                }

                guildAccount.channels.zombieChannels.Remove(channelID);
                guildAccount.channels.humanChannels.Remove(channelID);

                if (guildAccount.channels.generalAnnouncementChannel == channelID)
                    guildAccount.channels.generalAnnouncementChannel = 0;
                else if (guildAccount.channels.humanAnnouncementChannel == channelID)
                    guildAccount.channels.humanAnnouncementChannel = 0;
                else if (guildAccount.channels.zombieAnnouncementChannel == channelID)
                    guildAccount.channels.zombieAnnouncementChannel = 0;

                if (!guildAccount.channels.modChannels.Contains(channelID)) {
                    guildAccount.channels.modChannels.Add(channelID);
                    Accounts.SaveAccounts();
                    RoleHandler.UpdateChannel(channelID, guild);
                    await RespondAsync($":thumbsup: Added #{Context.Channel.Name} to mod channels :thumbsup:", ephemeral: true);
                    return;
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
                    if (guildAccount.channels.generalAnnouncementChannel == channelID) {
                        guildAccount.channels.generalAnnouncementChannel = 0;
                        Accounts.SaveAccounts();
                        await RespondAsync($":thumbsup: #{Context.Channel.Name} is no longer the general announcement channel :thumbsup:", ephemeral: true);
                    } else
                        await RespondAsync($":x: #{Context.Channel.Name} is not the general announcement channel :x:", ephemeral: true);

                    return;
                }

                guildAccount.channels.modChannels.Remove(channelID);
                guildAccount.channels.humanChannels.Remove(channelID);
                guildAccount.channels.zombieChannels.Remove(channelID);

                if (guildAccount.channels.humanAnnouncementChannel == channelID)
                    guildAccount.channels.humanAnnouncementChannel = 0;
                else if (guildAccount.channels.zombieAnnouncementChannel == channelID)
                    guildAccount.channels.zombieAnnouncementChannel = 0;

                if (guildAccount.channels.generalAnnouncementChannel != channelID) {
                    guildAccount.channels.generalAnnouncementChannel = channelID;
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
                    if (guildAccount.channels.zombieAnnouncementChannel == channelID) {
                        guildAccount.channels.zombieAnnouncementChannel = 0;
                        Accounts.SaveAccounts();
                        await RespondAsync($":thumbsup: #{Context.Channel.Name} is no longer the zombie announcement channel :thumbsup:", ephemeral: true);
                    } else
                        await RespondAsync($":x: #{Context.Channel.Name} is not the zombie announcement channel :x:", ephemeral: true);

                    return;
                }

                guildAccount.channels.modChannels.Remove(channelID);
                guildAccount.channels.humanChannels.Remove(channelID);
                guildAccount.channels.zombieChannels.Remove(channelID);

                if (guildAccount.channels.humanAnnouncementChannel == channelID)
                    guildAccount.channels.humanAnnouncementChannel = 0;
                else if (guildAccount.channels.generalAnnouncementChannel == channelID)
                    guildAccount.channels.generalAnnouncementChannel = 0;

                if (guildAccount.channels.zombieAnnouncementChannel != channelID) {
                    guildAccount.channels.zombieAnnouncementChannel = channelID;
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
                    if (guildAccount.channels.humanAnnouncementChannel == channelID) {
                        guildAccount.channels.humanAnnouncementChannel = 0;
                        Accounts.SaveAccounts();
                        await RespondAsync($":thumbsup: #{Context.Channel.Name} is no longer the human announcement channel :thumbsup:", ephemeral: true);
                    } else
                        await RespondAsync($":x: #{Context.Channel.Name} is not the human announcement channel :x:", ephemeral: true);

                    return;
                }

                guildAccount.channels.modChannels.Remove(channelID);
                guildAccount.channels.humanChannels.Remove(channelID);
                guildAccount.channels.zombieChannels.Remove(channelID);

                if (guildAccount.channels.zombieAnnouncementChannel == channelID)
                    guildAccount.channels.zombieAnnouncementChannel = 0;
                else if (guildAccount.channels.generalAnnouncementChannel == channelID)
                    guildAccount.channels.generalAnnouncementChannel = 0;

                if (guildAccount.channels.humanAnnouncementChannel != channelID) {
                    guildAccount.channels.humanAnnouncementChannel = channelID;
                    Accounts.SaveAccounts();
                    RoleHandler.UpdateChannel(channelID, guild);
                    await RespondAsync($":thumbsup: Set #{Context.Channel.Name} as the human announcement channel :thumbsup:", ephemeral: true);
                    return;
                } else
                    await RespondAsync($":question: #{Context.Channel.Name} is already the human announcement channel :question:", ephemeral: true);
            } else
                await RespondAsync(":x: This command can't be used here :x:", ephemeral: true);
        }
    }
}
