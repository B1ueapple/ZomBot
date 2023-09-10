using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Net;
using System;
using System.Threading.Tasks;
using ZomBot.Data;
using Newtonsoft.Json;

namespace ZomBot.Commands {
    public class Status : InteractionModuleBase {
        [SlashCommand("status", "See a player's hvz related information.")]
        [UserCommand("Status")]
        [RequireContext(ContextType.Guild)]
        public async Task StatusCommand([Summary("User", "Who to check.")] SocketUser user = null) {
            var target = Accounts.GetUser(user ?? Context.User, Context.Guild);

            if (target.playerData == null) {
                if (user == null)
                    await RespondAsync(":x: You are not currently linked.", ephemeral: true);
				else
					await RespondAsync(":x: That player is not currently linked.", ephemeral: true);

				return;
            }

            var sender = Accounts.GetUser(Context.User, Context.Guild);
            string extendedData = "";

            EmbedBuilder embed = new EmbedBuilder();

            if (sender.playerData.access == "mod" || sender.playerData.access == "admin" || sender.playerData.access == "superadmin") {
                ExtendedPlayerData data = target.extendedPlayerData;

				using (WebClient client = new WebClient()) {
                    try {
						data = JsonConvert.DeserializeObject<EPDReturn>(client.DownloadString($"{Config.bot.hvzwebsite}/api/v2/admin/users/{target.playerData.id}?apikey={Config.bot.apikey}")).user;
						
                        if (data != null) {
                            target.extendedPlayerData = data;
                            Accounts.SaveAccounts();
                            extendedData = $"\nZombie ID: '{data.zombieId}'\nOZ: {data.oz}\nHuman IDs: ";

                            bool first = true;
                            foreach (HumanID id in data.humanIds) {
                                if (!first)
                                    extendedData += ", ";

                                extendedData += $"'{id.idString}' ({(id.active ? "active" : "inactive")})";
                                first = false;
                            }
                        }
                    } catch (WebException) {
                        if (!Program.websiteDown) {
                            Program.websiteDown = true;
                            Program.Warning($"Could not retieve data from {Config.bot.hvzwebsite}");
                        }

						if (data != null) {
							extendedData = $"\nZombie ID: '{data.zombieId}'\nOZ: {data.oz}\nHuman IDs: ";

							bool first = true;
							foreach (HumanID id in data.humanIds) {
								if (!first)
									extendedData += ", ";

								extendedData += $"'{id.idString}' ({(id.active ? "active" : "inactive")})";
								first = false;
							}
						}
					}
                }
            }

            embed.WithCurrentTimestamp()
                .WithAuthor(user ?? Context.User)
                .AddField("**Player Data**", $"Name: {target.playerData.name}\nTeam: {target.playerData.team}\nTags: {target.playerData?.humansTagged ?? 0}{extendedData}");

            await RespondAsync(embed: embed.Build(), ephemeral: true);
        }
    }
}
