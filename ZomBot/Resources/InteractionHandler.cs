using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Reflection;
using System.Threading.Tasks;
using ZomBot.Data;

namespace ZomBot.Resources {
    public class InteractionHandler {
        DiscordSocketClient _client;
        InteractionService _service;

        public async Task InitializeAsync(DiscordSocketClient client) {
            _client = client;
            _service = new InteractionService(client.Rest);

            await _service.AddModulesAsync(Assembly.GetEntryAssembly(), null);
            _client.SlashCommandExecuted += HandleSlashCommandAsync;
            _client.UserCommandExecuted += UserCommandHandler;
        }

        private async Task UserCommandHandler(SocketUserCommand cmd) { // convenience menu button
            if (!(cmd is SocketInteraction command) || cmd.User.IsBot) return;

            var context = new SocketInteractionContext(_client, command);
            var result = await _service.ExecuteCommandAsync(context, null);

            if (!result.IsSuccess) {
                await cmd.RespondAsync($":x: {result.ErrorReason} :x:", ephemeral: true);
            }
        }

        public async Task SetupAsync() {
            foreach (SocketGuild guild in _client.Guilds) {
                var g = Accounts.GetGuild(guild);

                if (g.setupComplete != true) {
                    await _service.RegisterCommandsToGuildAsync(guild.Id);
                    Console.WriteLine("Completed setup.");
                    g.setupComplete = true;
                }
            }
            Accounts.SaveAccounts();
        }

        private async Task HandleSlashCommandAsync(SocketSlashCommand cmd) {
            if (!(cmd is SocketInteraction command) || cmd.User.IsBot) return;

            var context = new SocketInteractionContext(_client, command);
            var result = await _service.ExecuteCommandAsync(context, null);

            if (!result.IsSuccess) {
                Console.WriteLine(result.ErrorReason);
                await cmd.RespondAsync($":x: {result.ErrorReason} :x:", ephemeral: true);
            }
        }
    }
}
