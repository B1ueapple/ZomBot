using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZomBot.Data;

namespace ZomBot.Commands {
    public class Refresh : InteractionModuleBase {
        [SlashCommand("refresh", "Refresh player list.")]
        [RequireContext(ContextType.Guild)]
        [DefaultMemberPermissions(GuildPermission.ManageRoles)]
        public async Task RefreshListCommand() {
            if (Context.Guild is SocketGuild guild) {
                var guildAccount = Accounts.GetGuildAccount(guild.Id);
                string list = "", listUnlinked = "";

                foreach (UserData user in guildAccount.userData) {
                    if (user.playerData.name == null || user.playerData.name == "")
                        continue;

                    list += user.playerData.name;
                    list += "\n";
                }

                foreach (PlayerData data in Program.getPDL().players) {
                    if (list.Contains(data.name))
                        continue;

                    listUnlinked += data.name;
                    listUnlinked += "\n";
                }

                if (list.Length > 2)
                    list.Substring(0, list.Length - 2);
                if (listUnlinked.Length > 2)
                    listUnlinked.Substring(0, listUnlinked.Length - 2);

                await RespondAsync($":thumbsup: Refreshed :thumbsup:", ephemeral: true);
            } else
                await RespondAsync(":x: This command can't be used here :x:", ephemeral: true);
        }
    }
}
