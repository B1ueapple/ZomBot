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

            await RespondAsync(":white_check_mark: Refreshed Website Data.", ephemeral: true);
        }
    }
}
