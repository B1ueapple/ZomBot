using Discord;
using Discord.Interactions;
using Discord.WebSocket;
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
                if (!Config.bot.apionline) {
                    await RespondAsync(":x: Linking is currently disabled :x:", ephemeral: true);
                    return;
				}

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
            if (!Config.bot.apionline) {
                await RespondAsync(":x: Linking is currently disabled :x:", ephemeral: true);
                return;
            }

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
            var linkbutton = new ButtonBuilder() {
                Label = "Link Account",
                CustomId = "link_button",
                Style = ButtonStyle.Primary
            };

            var ozbutton = new ButtonBuilder() {
                Label = "Apply for OZ",
                CustomId = "oz_button",
                Style = ButtonStyle.Danger
            };

            var component = new ComponentBuilder();
            component.WithButton(linkbutton);
            component.WithButton(ozbutton);

            await RespondAsync(components: component.Build());
        }

        [ComponentInteraction("link_button")]
        public async Task HandleLinkButton() {
            if (!Config.bot.apionline) {
                await RespondAsync(":x: Linking is currently disabled :x:", ephemeral: true);
                return;
            }

            await RespondWithModalAsync<LinkModal>("link_modal");
		}

        [ComponentInteraction("oz_button")]
        public async Task HandleOZButton() {
            var user = Accounts.GetUser(Context.User, Context.Guild);

            if (user?.playerData?.name != null) {
                if (!(user?.ozApp.applied ?? false)) {
                    await RespondWithModalAsync<OZModal>("oz_modal");
                } else {
                    await RespondAsync(":x: You have already submitted an application :x:", ephemeral: true);
				}
			} else {
                await RespondAsync(":x: You are not linked! Use the link button to link your account first :x:", ephemeral: true);
			}
		}

        [ModalInteraction("link_modal")]
        public async Task HandleLinkMenu(LinkModal menu) {
            string name = menu.Name.Trim();

            if (!Config.bot.apionline) {
                await RespondAsync(":x: Linking is currently disabled :x:", ephemeral: true);
                return;
            }

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

        [ModalInteraction("oz_modal")]
        public async Task HandleOZMenu(OZModal menu) {
            string rating = menu.Rating.Trim();
            string time = menu.Time.Trim();
            string experience = menu.Experience.Trim();
            string why = menu.Why.Trim();

            var guild = Accounts.GetGuild(Context.Guild);
            var user = Accounts.GetUser(Context.User, Context.Guild);

            if (user.ozApp.applied) {
                await RespondAsync(":x: You have already submitted an application :x:");
                return;
			}

            user.ozApp = new OZApplication() {
                applied = true,
                rating = rating,
                time = time,
                experience = experience,
                why = why
            };

            var mic = await Context.Guild.GetTextChannelAsync(guild.channels.modImportantChannel);

            var eb = new EmbedBuilder()
                .WithCurrentTimestamp()
                .WithAuthor(Context.User)
                .WithTitle("**OZ Application**")
                .AddField("Website Name (Discord Username)", $"{user.playerData.name} ({Context.User.Username})")
                .AddField("How eager are you to be OZ on a scale of 1-10 (10 is the highest)", rating)
                .AddField("When do you plan to start playing on Monday?", time)
                .AddField("How many semesters of HvZ have you played?", experience)
                .AddField("Why would you like to be an OZ?", why == "" ? "No answer given." : why);
            
            if (mic != null)
                await mic.SendMessageAsync(embed: eb.Build());
            
            Accounts.SaveAccounts();
            await RespondAsync($":thumbsup: Application submitted. Remember that if you are chosen as an OZ, you are expected to commit heavily to playing until a at least a few tags are made. :thumbsup:", ephemeral: true);
        }
    }

    public class LinkModal : IModal {
        public string Title => "Link Account";
        
        [InputLabel("Your name (as it appears on the website)")]
        [ModalTextInput("name_input", TextInputStyle.Short, "Not case sensitive.", 3, 50)]
        public string Name { get; set; }
	}

    public class OZModal : IModal {
        public string Title => "OZ Application";
        // 45 char label limit??? whack.
        [InputLabel("How eager are you to be OZ on a scale of 1-10")]
        [ModalTextInput("rating_input", TextInputStyle.Short, "10 is the highest.", 1)]
        public string Rating { get; set; }

        [InputLabel("When do you plan to start playing on Monday?")]
        [ModalTextInput("time_input", TextInputStyle.Short, "Assuming the game starts at 6am.", 1)]
        public string Time { get; set; }

        [InputLabel("How many semesters of HvZ have you played?")]
        [ModalTextInput("exp_input", TextInputStyle.Short, "0? 5? 9354?", 1)]
        public string Experience { get; set; }

        [InputLabel("Why would you like to be an OZ?")]
        [ModalTextInput("why_input", TextInputStyle.Paragraph, "Optional.", 0, 1000)]
        public string Why { get; set; }
    }
}
