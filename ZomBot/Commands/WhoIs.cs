using Discord.Interactions;
using Discord.WebSocket;
using System.Threading.Tasks;
using ZomBot.Data;

namespace ZomBot.Commands {
    public class WhoIs : InteractionModuleBase {
        [SlashCommand("whois", "See a player's hvz related information.")]
        [UserCommand("Whois")]
        [RequireContext(ContextType.Guild)]
        public async Task WhoIsCommand([Summary("User", "Who to check.")] SocketUser user) {
            var account = Accounts.GetUser(user.Id, Context.Guild.Id);

            if (account.playerData.name != null)
                await RespondAsync(account.playerData.name, ephemeral: true);
            else 
                 await RespondAsync(":x: That user is not linked :x:", ephemeral: true);
        }

        [SlashCommand("find", "Find a player's discord by their hvz name.")]
        [RequireContext(ContextType.Guild)]
        public async Task FindCommand([Summary("Name", "User's name on the website.")] string name) {
            var guild = Accounts.GetGuild(Context.Guild.Id);

            foreach (UserData ud in guild.userData) {
                if (ud.playerData.name != null) {
                    if (ud.playerData.name.ToLower().Contains(name.ToLower())) {
                        var user = await Context.Guild.GetUserAsync(ud.id);
                        await RespondAsync(user.Username, ephemeral: true);
                        return;
                    }
                }
            }

            await RespondAsync(":x: That player is not linked :x:", ephemeral: true);
        }
    }
}
