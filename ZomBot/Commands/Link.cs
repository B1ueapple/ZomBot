using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;
using ZomBot.Data;
using ZomBot.Resources;

namespace ZomBot.Commands {
    public class Link : InteractionModuleBase {
        [SlashCommand("link", "Link your discord to hvz. You must be registered on the website first.")]
        [RequireContext(ContextType.Guild)]
        public async Task LinkCommand([Summary("Name", "Use your name as spelled on the website. (Not case sensitive)")] string name) {
            if (Context.Guild is SocketGuild guild) {
                var ud = Accounts.GetUser(Context.User.Id, guild.Id);
                // There is absolutely nothing stopping multiple people from linking to the same account.
                if (ud.playerData.name != null) {
                    if (ud.playerData.name.ToLower().Contains(name.ToLower())) {
                        await RespondAsync(":x: You are already linked :x:", ephemeral: true);
                        return;
                    }
                }

                { // player list check
                    var result = from a in Program.getPDL().players
                                 where a.name.ToLower().Contains(name.ToLower())
                                 select a;

                    var player = result.FirstOrDefault();
                    if (player != null) {
                        ud.playerData = player;
                        Accounts.SaveAccounts();

                        if (!ud.blacklisted) {
                            if (player.team == "human") {
                                await RoleHandler.JoinHumanTeam(Context.User, guild);

                                if (ud.playerData.clan != null && ud.playerData.clan != "")
                                    await RoleHandler.JoinClan(Context.User, guild, ud.playerData.clan);
                            } else if (player.team == "zombie")
                                await RoleHandler.JoinZombieTeam(Context.User, guild);
                        }

                        Program.Log($"{player.name} has linked their discord.");
                        await RespondAsync($":thumbsup: You have successfully linked your account to {player.name} :thumbsup:", ephemeral: true);
                        return;
                    }
                }
                { // mod list check
                    var result = from a in Program.getMDL().players
                                 where a.name.ToLower().Contains(name.ToLower())
                                 select a;

                    var player = result.FirstOrDefault();
                    if (player != null) {
                        ud.playerData = player;
                        Accounts.SaveAccounts();

                        if (!ud.blacklisted)
                            await RoleHandler.JoinModTeam(Context.User, guild);

                        Program.Log($"{player.name} has linked their discord.");
                        await RespondAsync($":thumbsup: You have successfully linked your account to {player.name} :thumbsup:", ephemeral: true);
                        return;
                    }
                }

                await RespondAsync($":x: Could not find {name}, check spelling and ensure you appear on the website :x:", ephemeral: true);
            } else
                await RespondAsync(":x: This command can't be used here :x:", ephemeral: true);
        }

        [SlashCommand("unlink", "Unlink your discord from hvz. Does nothing if you aren't currently linked.")]
        [UserCommand("Unlink")]
        [RequireContext(ContextType.Guild)]
        public async Task UnlinkCommand([Summary("User", "The discord user to unlink. (Defaults to yourself)")] SocketUser user = null) {
            if (Context.Guild is SocketGuild guild) {
                if (user != null) {
                    var sender = await Context.Guild.GetUserAsync(Context.User.Id);

                    if (!sender.GuildPermissions.ManageRoles) {
                        await RespondAsync(":x: You don't have permission to do that :x:", ephemeral: true);
                        return;
                    }
                }

                var ud = Accounts.GetUser(user ?? Context.User, guild);

                if (ud.playerData.name != null && ud.playerData.name != "") {
                    Program.Log($"{ud.playerData.name} has unlinked their account.");
                    PlayerData p = new PlayerData {
                        name = ""
                    };

                    ud.playerData = p;

                    await RoleHandler.LeaveTeams(user ?? Context.User, guild);
                    await RespondAsync($":thumbsup: You have successfully unlinked {(user != null ? (user.Username + "'s") : "your")} account :thumbsup:", ephemeral: true);
                    return;
                }

                await RespondAsync(":x: You aren't linked :x:", ephemeral: true);
            } else
                await RespondAsync(":x: This command can't be used here :x:", ephemeral: true);
        }

        [SlashCommand("linkother", "Link someone's discord to hvz. They must be registered on the website first.")]
        [RequireContext(ContextType.Guild)]
        [DefaultMemberPermissions(GuildPermission.ManageRoles)]
        public async Task LinkOtherCommand([Summary("User", "The discord user to link.")] SocketUser user, [Summary("Name", "Their name as spelled on the website. (Not case sensitive)")] string name) {
            if (Context.Guild is SocketGuild guild) {
                var ud = Accounts.GetUser(user.Id, guild.Id);

                if (ud.playerData.name != null) {
                    if (ud.playerData.name.ToLower().Contains(name.ToLower())) {
                        await RespondAsync(":x: They are already linked :x:", ephemeral: true);
                        return;
                    }
                }
                { // player list check
                    var result = from a in Program.getPDL().players
                                 where a.name.ToLower().Contains(name.ToLower())
                                 select a;

                    var player = result.FirstOrDefault();
                    if (player != null) {
                        ud.playerData = player;
                        Accounts.SaveAccounts();

                        if (!ud.blacklisted) {
                            if (player.team == "human")
                                await RoleHandler.JoinHumanTeam(user, guild);
                            else if (player.team == "zombie")
                                await RoleHandler.JoinZombieTeam(user, guild);
                        }

                        Program.Log($"{player.name} has been linked by {Context.User.Username}.");
                        await RespondAsync($":thumbsup: You have successfully linked their account to {player.name} :thumbsup:", ephemeral: true);
                        return;
                    }
                }
                { // mod list check
                    var result = from a in Program.getMDL().players
                                 where a.name.ToLower().Contains(name.ToLower())
                                 select a;

                    var player = result.FirstOrDefault();
                    if (player != null) {
                        ud.playerData = player;
                        Accounts.SaveAccounts();

                        if (!ud.blacklisted)
                            await RoleHandler.JoinModTeam(user, guild);

                        Program.Log($"{player.name} has been linked by {Context.User.Username}.");
                        await RespondAsync($":thumbsup: You have successfully linked their account to {player.name} :thumbsup:", ephemeral: true);
                        return;
                    }
                }

                await RespondAsync($":x: Could not find {name}, check spelling and ensure they appear on the website :x:", ephemeral: true);
            } else
                await RespondAsync(":x: This command can't be used here :x:", ephemeral: true);
        }

        [SlashCommand("checklinked", "List all linked and unlinked players.")]
        [RequireContext(ContextType.Guild)]
        [DefaultMemberPermissions(GuildPermission.ManageRoles)]
        public async Task CheckLinkedCommand() {
            if (Context.Guild is SocketGuild guild) {
                var guildAccount = Accounts.GetGuild(guild.Id);
                string list = "", listUnlinked = "", dupes = "", dupes2electricboogaloo = "";

                foreach (UserData user in guildAccount.userData) {
                    if (user.playerData.name == null || user.playerData.name == "")
                        continue;

                    if (list.Contains("+ " + user.playerData.name)) {
                        dupes += "~ " + user.playerData.name;
                        dupes += "\n";
                    }

                    list += "+ " + user.playerData.name;
                    list += "\n";
                }

                foreach (PlayerData data in Program.getPDL().players) {
                    if (list.Contains("+ " + data.name))
                        continue;

                    listUnlinked += "- " + data.name;
                    listUnlinked += "\n";
                }

                foreach (UserData user in guildAccount.userData) {
                    if (user.playerData.name == null || user.playerData.name == "")
                        continue;

                    if (dupes.Contains("~ " + user.playerData.name)) {
                        dupes2electricboogaloo += "~ " + user.playerData.name + " = " + guild.GetUser(user.id);
                        dupes2electricboogaloo += "\n";
                    }
                }

                if (list.Length > 2)
                    list.Substring(0, list.Length - 2);

                if (listUnlinked.Length > 2)
                    listUnlinked.Substring(0, listUnlinked.Length - 2);

                if (dupes2electricboogaloo.Length > 2)
                    dupes2electricboogaloo.Substring(0, dupes2electricboogaloo.Length - 2);
                //                                                              v makes the list too long for discord
                await RespondAsync($"```\nMISSING\n{listUnlinked}```" /*+ "\n```\nPRESENT\n{list}```"*/ + (dupes2electricboogaloo.Length > 0 ? $"\n```\nDUPLICATE LINKS\n{dupes2electricboogaloo}```" : ""), ephemeral: true);
            } else
                await RespondAsync(":x: This command can't be used here :x:", ephemeral: true);
        }

        [SlashCommand("linkbutton", "Send a message with a button to make linking simpler.")]
        [RequireContext(ContextType.Guild)]
        [DefaultMemberPermissions(GuildPermission.ManageRoles)]
        public async Task LinkButtonCommand() {
            var button = new ButtonBuilder() {
                Label = "Link Account",
                CustomId = "link_button",
                Style = ButtonStyle.Primary
            };

            var component = new ComponentBuilder();
            component.WithButton(button);

            await RespondAsync(components: component.Build());
        }

        [ComponentInteraction("link_button")]
        public async Task HandleLinkButton() {
            await RespondWithModalAsync<LinkModal>("link_modal");
		}

        [ModalInteraction("link_modal")]
        public async Task HandleLinkMenu(LinkModal menu) {
            string name = menu.Name;

            if (Context.Guild is SocketGuild guild) {
                var ud = Accounts.GetUser(Context.User.Id, guild.Id);
                // There is absolutely nothing stopping multiple people from linking to the same account.
                if (ud.playerData.name != null) {
                    if (ud.playerData.name.ToLower().Contains(name.ToLower())) {
                        await RespondAsync(":x: You are already linked :x:", ephemeral: true);
                        return;
                    }
                }

                { // player list check
                    var result = from a in Program.getPDL().players
                                 where a.name.ToLower().Contains(name.ToLower())
                                 select a;

                    var player = result.FirstOrDefault();
                    if (player != null) {
                        ud.playerData = player;
                        Accounts.SaveAccounts();

                        if (!ud.blacklisted) {
                            if (player.team == "human") {
                                await RoleHandler.JoinHumanTeam(Context.User, guild);

                                if (ud.playerData.clan != null && ud.playerData.clan != "")
                                    await RoleHandler.JoinClan(Context.User, guild, ud.playerData.clan);
                            } else if (player.team == "zombie")
                                await RoleHandler.JoinZombieTeam(Context.User, guild);
                        }

                        Program.Log($"{player.name} has linked their discord.");
                        await RespondAsync($":thumbsup: You have successfully linked your account to {player.name} :thumbsup:", ephemeral: true);
                        return;
                    }
                }
                { // mod list check
                    var result = from a in Program.getMDL().players
                                 where a.name.ToLower().Contains(name.ToLower())
                                 select a;

                    var player = result.FirstOrDefault();
                    if (player != null) {
                        ud.playerData = player;
                        Accounts.SaveAccounts();

                        if (!ud.blacklisted)
                            await RoleHandler.JoinModTeam(Context.User, guild);

                        Program.Log($"{player.name} has linked their discord.");
                        await RespondAsync($":thumbsup: You have successfully linked your account to {player.name} :thumbsup:", ephemeral: true);
                        return;
                    }
                }

                await RespondAsync($":x: Could not find {name}, check spelling and ensure you appear on the website :x:", ephemeral: true);
            } else
                await RespondAsync(":x: How did we get here??? :x:", ephemeral: true);
        }
    }

    public class LinkModal : IModal {
        public string Title => "Link Account";
        
        [InputLabel("Website Name")]
        [ModalTextInput("name_input", TextInputStyle.Short, "Not case sensitive.", 3, 50)]
        public string Name { get; set; }
	}
}
