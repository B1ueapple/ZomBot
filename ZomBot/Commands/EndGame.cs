using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Threading.Tasks;
using ZomBot.Data;
using ZomBot.Resources;

namespace ZomBot.Commands {
    public class EndGame : InteractionModuleBase {
        [SlashCommand("endgame", "End the game. (Doesn't affect the website)")]
        [RequireContext(ContextType.Guild)]
        [DefaultMemberPermissions(GuildPermission.ManageGuild)]
        public async Task EndGameCommand() {
            var account = Accounts.GetUser(Context.User, Context.Guild);

            if (!(account.playerData.access == "mod" || account.playerData.access == "admin" || account.playerData.access == "superadmin")) {
                await RespondAsync($":x: You must be a mod to perform this action :x:", ephemeral: true);
                return;
            }

            if (Context.Guild is SocketGuild g) {
                await RespondAsync(":thumbsup: The server is being updated to reflect the game ending :thumbsup:", ephemeral: true); // respond before execution because execution takes too long...
                await RoleHandler.EndGame(g);
            } else
                await RespondAsync(":x: This command can't be used here :x:", ephemeral: true);
        }
    }
}
