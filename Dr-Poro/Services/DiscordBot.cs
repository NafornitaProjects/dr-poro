using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;

namespace Dr_Poro.Services
{
    public class DiscordBot : BackgroundService
    {
        private readonly DiscordSocketClient _client;
        private readonly SlashCommandHandler _commandHandler;
        private readonly ISlashCommandRegistry _commandRegistry;

        public DiscordBot(DiscordSocketClient client, ISlashCommandRegistry commandRegistry, SlashCommandHandler commandHandler)
        {
            _commandHandler = commandHandler;
            _commandRegistry = commandRegistry;
            _client = client;

            _client.Ready += async () =>
            {
                await _commandRegistry.RegisterCommandsAsync();
            };
            _client.SlashCommandExecuted += _commandHandler.HandleCommandAsync;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            string token = Environment.GetEnvironmentVariable("DISCORD_TOKEN")
                ?? throw new Exception("DISCORD_TOKEN environment variable not found");
            
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}
