using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.IO;
using System.Threading.Tasks;
using ZomBot.Data;
using ZomBot.Resources;

namespace ZomBot.Commands {
    public class EndGame : InteractionModuleBase {
        [SlashCommand("endgame", "End the game. (Doesn't affect the website)")]
        [RequireContext(ContextType.Guild)]
        [DefaultMemberPermissions(GuildPermission.ManageGuild)]
        public async Task EndGameCommand([Summary("Survivors", "If there were survivors. (Setting to false will ensure everyone \"dies\" on the last day.)")] bool survivors) {
            var account = Accounts.GetUser(Context.User, Context.Guild);

            if (!(account.playerData.access == "mod" || account.playerData.access == "admin" || account.playerData.access == "superadmin")) {
                await RespondAsync($":x: You must be a mod to perform this action :x:", ephemeral: true);
                return;
            }

            var guildAccount = Accounts.GetGuild(Context.Guild);
            if (!guildAccount.gameData.active) {
                await RespondAsync(":x: No active game to end :x:", ephemeral: true);
                return;
            }

            if (Context.Guild is SocketGuild g) {
                await RespondAsync(":thumbsup: The server is being updated to reflect the game ending :thumbsup:", ephemeral: true);
                await RoleHandler.EndGame(g, survivors);

                if (!Directory.Exists("Data"))
                    Directory.CreateDirectory("Data");

                var log = guildAccount.gameLog.GetFormattedMessages(GameLogMessageFormat.WEBSITE);
                File.WriteAllText("Data/GameLog.txt", log);

                await Context.Channel.SendFileAsync(new FileAttachment("Data/GameLog.txt"));
            } else
                await RespondAsync(":x: This command can't be used here :x:", ephemeral: true);
        }
    }
}
