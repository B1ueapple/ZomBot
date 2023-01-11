using Discord;
using Discord.Interactions;
using System.Threading.Tasks;
using ZomBot.Data;

namespace ZomBot.Commands {
	public class StartGame : InteractionModuleBase {
		[SlashCommand("startgame", "Start the game. (Doesn't affect the website)")]
		[RequireContext(ContextType.Guild)]
		[DefaultMemberPermissions(GuildPermission.ManageGuild)]
		public async Task StartGameCommand() {
			var account = Accounts.GetUser(Context.User, Context.Guild);

			if (!(account.playerData.access == "mod" || account.playerData.access == "admin" || account.playerData.access == "superadmin")) {
				await RespondAsync($":x: You must be a mod to perform this action :x:", ephemeral: true);
				return;
			}

			var guildAccount = Accounts.GetGuild(Context.Guild);

			if (!guildAccount.gameData.active) {
				await RespondAsync(":x: Game already ongoing :x:", ephemeral: true);
				return;
			}

			guildAccount.gameLog.StartMessage();
			guildAccount.gameData.active = true;
			guildAccount.gameData.startTime = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
			Accounts.SaveAccounts();

			await RespondAsync(":thumbsup: The game has been marked as started :thumbsup:", ephemeral: true);
		}
	}
}
