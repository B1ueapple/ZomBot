using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Threading.Tasks;
using ZomBot.Data;

namespace ZomBot.Commands {
    public class Warn : InteractionModuleBase {
        [SlashCommand("warn", "Warn a user.")]
        [RequireContext(ContextType.Guild)]
        [DefaultMemberPermissions(GuildPermission.ManageGuild)]
        public async Task WarnCommand([Summary("User", "Who to warn.")] SocketUser user, [Summary("Reason", "Why the warning was issued")] string reason) {
            var account = Accounts.GetUser(user, Context.Guild);
            account.AddWarning(Context.User, reason);
            Accounts.SaveAccounts();

            await RespondAsync($"Added warning for {user.Username}.", ephemeral: true);
        }

        [UserCommand("Warnings")]
        [RequireContext(ContextType.Guild)]
        [DefaultMemberPermissions(GuildPermission.ManageGuild)]
        public async Task WarningsCommand([Summary("User", "Who to check.")] SocketUser user) {
            var account = Accounts.GetUser(user, Context.Guild);
            EmbedBuilder embed = new EmbedBuilder();

            embed.WithCurrentTimestamp()
                .WithAuthor(user);

            if ((account.warnings?.Count ?? 0) > 0) {
                foreach (Warning w in account.warnings)
                    embed.AddField($"**{System.DateTimeOffset.FromUnixTimeMilliseconds(w.time)}**", $"{Context.Guild.GetUserAsync(w.issuer).Result.DisplayName}: {w.reason}");
            } else
                embed.AddField("N/A", "Squeaky clean record.");

            await RespondAsync(embed: embed.Build(), ephemeral: true);
        }
    }
}
