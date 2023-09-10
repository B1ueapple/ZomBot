using Discord;
using Discord.Interactions;
using System.Threading.Tasks;

namespace ZomBot.Commands {
    public class Refresh : InteractionModuleBase {
        [SlashCommand("refresh", "Force a recheck of everyone's roles.")]
        [RequireContext(ContextType.Guild)]
        [DefaultMemberPermissions(GuildPermission.ManageGuild)]
        public async Task RefreshCommand() {
            Program.overrideCheck = true;

            await RespondAsync(":white_check_mark: Enabled override for next sweep.", ephemeral: true);
        }
    }
}
