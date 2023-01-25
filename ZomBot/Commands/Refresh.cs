using Discord;
using Discord.Interactions;
using System.Threading.Tasks;

namespace ZomBot.Commands {
    public class Refresh : InteractionModuleBase {
        [SlashCommand("refresh", "Refresh website data.")]
        [RequireContext(ContextType.Guild)]
        [DefaultMemberPermissions(GuildPermission.ManageGuild)]
        public async Task RefreshCommand() {
            Program.GetSiteData();

            await RespondAsync(":thumbsup: Refreshed Website Data :thumbsup:", ephemeral: true);
        }
    }
}
