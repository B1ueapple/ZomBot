using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using ZomBot.Data;

namespace ZomBot.Commands {
    public class Status : InteractionModuleBase {
        [SlashCommand("status", "See a player's hvz related information.")]
        [UserCommand("Status")]
        [RequireContext(ContextType.Guild)]
        public async Task StatusCommand([Summary("User", "Who to check.")] SocketUser user = null) {
            var target = Accounts.GetUser(user ?? Context.User, Context.Guild);
            var sender = Accounts.GetUser(Context.User, Context.Guild);
            string Warnings = "";
            EmbedBuilder embed = new EmbedBuilder();

            if (sender.playerData.access == "mod" || sender.playerData.access == "admin" || sender.playerData.access == "superadmin")
                Warnings = $"*Warnings: {target.warnings.Count}*\n";

            embed.WithCurrentTimestamp()
                .WithAuthor(user ?? Context.User)
                .AddField("**Player Data**", $"Name: {target.playerData.name}\nTeam: {target.playerData.team}\n# of Tags: {target.playerData?.humansTagged ?? 0}\n{Warnings}");

            await RespondAsync(embed: embed.Build(), ephemeral: true);
        }
    }
}
