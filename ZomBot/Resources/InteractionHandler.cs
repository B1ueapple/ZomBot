using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Reflection;
using System.Threading.Tasks;
using ZomBot.Data;

namespace ZomBot.Resources {
    public class InteractionHandler {
        private readonly DiscordSocketClient _client;
        private readonly InteractionService _commands;

        public InteractionHandler(DiscordSocketClient client) {
            _client = client;
            _commands = new InteractionService(_client.Rest);
		}

        public async Task InitializeAsync() {
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), null);
            //_client.SlashCommandExecuted += HandleSlashCommandAsync;
            //_client.UserCommandExecuted += UserCommandHandler;
            _client.InteractionCreated += HandleInteraction;
        }

        private async Task HandleInteraction(SocketInteraction interaction) {
            try {
                var context = new SocketInteractionContext(_client, interaction);
                var result = await _commands.ExecuteCommandAsync(context, null);


                if (!result.IsSuccess)
                    await interaction.RespondAsync($":x: {result.ErrorReason} :x:", ephemeral: true);
            } catch (Exception e) {
                await interaction.RespondAsync($":x: {e.Message} :x:", ephemeral: true);
            }
		}
        /*
        private async Task UserCommandHandler(SocketUserCommand cmd) { // convenience menu button
            if (!(cmd is SocketInteraction command) || cmd.User.IsBot) return;

            var context = new SocketInteractionContext(_client, command);
            var result = await _commands.ExecuteCommandAsync(context, null);

            if (!result.IsSuccess) {
                await cmd.RespondAsync($":x: {result.ErrorReason} :x:", ephemeral: true);
            }
        }
        
        private async Task HandleSlashCommandAsync(SocketSlashCommand cmd) {
            if (!(cmd is SocketInteraction command) || cmd.User.IsBot) return;

            var context = new SocketInteractionContext(_client, command);
            var result = await _commands.ExecuteCommandAsync(context, null);

            if (!result.IsSuccess) {
                Console.WriteLine(result.ErrorReason);
                await cmd.RespondAsync($":x: {result.ErrorReason} :x:", ephemeral: true);
            }
        }
        */
        public async Task SetupAsync() {
            foreach (SocketGuild guild in _client.Guilds) {
                var g = Accounts.GetGuild(guild);

                if (g.setupComplete != true) {
                    await _commands.RegisterCommandsToGuildAsync(guild.Id);
                    Console.WriteLine("Completed setup.");
                    g.setupComplete = true;
                }
            }
            Accounts.SaveAccounts();
        }
    }
}
