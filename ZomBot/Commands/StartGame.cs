using Discord;
using Discord.Interactions;
using System.Threading.Tasks;
using ZomBot.Data;

namespace ZomBot.Commands {
	public class StartGame : InteractionModuleBase {
		[SlashCommand("startgame", "Start the game. (Doesn't affect the website)")]
		[RequireContext(ContextType.Guild)]
		[DefaultMemberPermissions(GuildPermission.ManageGuild)]
		public async Task EndGameCommand([Summary("Survivors", "If there were survivors. (Setting to false will ensure everyone \"dies\" on the last day.)")] bool survivors) {
			var account = Accounts.GetUser(Context.User, Context.Guild);

			if (!(account.playerData.access == "mod" || account.playerData.access == "admin" || account.playerData.access == "superadmin")) {
				await RespondAsync($":x: You must be a mod to perform this action :x:", ephemeral: true);
				return;
			}

			var guildAccount = Accounts.GetGuild(Context.Guild);
			guildAccount.gameData.active = true;
			guildAccount.gameData.startTime = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();

			await RespondAsync(":thumbsup: The server has been updated to reflect the game ending :thumbsup:", ephemeral: true);
		}
	}
}
