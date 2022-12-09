using Discord.Interactions;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;
using ZomBot.Data;

namespace ZomBot.Commands {
    public class ClanColor : InteractionModuleBase {
        [SlashCommand("clancolor", "Change the color of your clan.")]
        [RequireContext(ContextType.Guild)]
        public async Task ClanColorCommand([Summary("Color", "What color to set your clan tag to.")] uint color) {
            var guild = Accounts.GetGuild(Context.Guild.Id);
            var user = Accounts.GetUser(Context.User, Context.Guild);

            var result = from c in guild.clanList
                         where c.clanName.ToLower() == user.playerData.clan.ToLower()
                         select c;

            Clan clan = result.FirstOrDefault();

            if (clan.clanName == "" || clan.clanName == null)
                await RespondAsync(":x: You're not in a clan :x:", ephemeral: true);

            if (Context.Guild is SocketGuild g) {
                await g.GetRole(clan.roleID).ModifyAsync(x => x.Color = new Discord.Color(color));
                await RespondAsync(":thumbsup: You have updated your clan's color :thumbsup:", ephemeral: true);
            } else
                await RespondAsync(":x: This command can't be used here :x:", ephemeral: true);
        }
    }
}
