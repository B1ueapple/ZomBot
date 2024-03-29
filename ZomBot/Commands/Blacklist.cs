﻿using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Threading.Tasks;
using ZomBot.Data;

namespace ZomBot.Commands {
    public class Blacklist : InteractionModuleBase {
        [SlashCommand("blacklist", "Toggles whether or not Zombot will automagically update a specific user.")]
        [RequireContext(ContextType.Guild)]
        [DefaultMemberPermissions(GuildPermission.ManageRoles)]
        public async Task BlacklistCommand([Summary("User", "Who to modify.")] SocketUser user, [Summary("Status", "What to set their status to. (Defaults to true)")] bool set = true) {
            var account = Accounts.GetUser(user, Context.Guild);
            account.blacklisted = set;
            Accounts.SaveAccounts();

			Program.Info($"{(set ? "Added" : "Removed")} {user.Username} {(set ? "to" : "from")} blacklist.");
			await RespondAsync($":white_check_mark: {(set ? "Added" : "Removed")} {user.Username} {(set ? "to" : "from")} blacklist.", ephemeral: true);
        }

        [UserCommand("Blacklist")]
        [RequireContext(ContextType.Guild)]
        [DefaultMemberPermissions(GuildPermission.ManageGuild)]
        public async Task BlacklistCommand([Summary("User", "Who to modify.")] SocketUser user) {
            var account = Accounts.GetUser(user, Context.Guild);
            account.blacklisted = !account.blacklisted;
            Accounts.SaveAccounts();

			Program.Info($"{(account.blacklisted ? "Added" : "Removed")} {user.Username} {(account.blacklisted ? "to" : "from")} blacklist.");
			await RespondAsync($":white_check_mark: {(account.blacklisted ? "Added" : "Removed")} {user.Username} {(account.blacklisted ? "to" : "from")} blacklist.", ephemeral: true);
        }
    }
}
