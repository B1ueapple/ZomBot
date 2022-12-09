using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ZomBot.Data;

namespace ZomBot.Resources {
    public class CommandHandler {
        DiscordSocketClient _client;
        CommandService _service;

        public async Task InitializeAsync(DiscordSocketClient client) {
            _client = client;
            _service = new CommandService();

            await _service.AddModulesAsync(Assembly.GetEntryAssembly(), null);
            _client.MessageReceived += HandleCommandAsync;
        }

        private async Task HandleCommandAsync(SocketMessage s) {
            if (!(s is SocketUserMessage msg) || s.Author.IsBot) return;

            var context = new SocketCommandContext(_client, msg);

            if (context.Guild != null) {
                int argPos = 0;

                if (msg.HasStringPrefix(Config.bot.prefix, ref argPos) || msg.HasMentionPrefix(_client.CurrentUser, ref argPos)) {
                    var result = await _service.ExecuteAsync(context, argPos, null);

                    if (!result.IsSuccess && result.Error != CommandError.UnknownCommand) {
                        Console.WriteLine(result.ErrorReason);
                        await context.Channel.SendMessageAsync($":x: {result.ErrorReason} :x:");
                    }
                }
            } else
                await context.Channel.SendMessageAsync("I'm afraid of isolated spaces. (My owner is too lazy to fix crashes caused by using commands in DMs)");
        }
    }
}
