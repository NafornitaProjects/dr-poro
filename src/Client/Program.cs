using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DotNetEnv;
using Microsoft.Extensions.Logging;

using DrPoro.Application;
using DrPoro.Client.InteractionModules;
using DrPoro.Infrastructure;

namespace DrPoro.Client;

public class Program
{
    public static async Task Main(string[] args)
    {
        Env.Load();
        
        var configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();

        var services = new ServiceCollection();

        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.SetMinimumLevel(LogLevel.Information);
            loggingBuilder.AddConsole();
        });

        services.AddInfrastructureServices(configuration);

        services.AddApplicationServices();

        services.AddSingleton<DiscordSocketClient>();
        
        services.AddSingleton<InteractionService>();
        
        services.AddTransient<RestartInteractionModule>();

        await using var serviceProvider = services.BuildServiceProvider();

        var client = serviceProvider.GetRequiredService<DiscordSocketClient>();
        var interactionService = serviceProvider.GetRequiredService<InteractionService>();
        var settings = serviceProvider.GetRequiredService<IOptions<DiscordSettings>>().Value;
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        await interactionService.AddModulesAsync(typeof(RestartInteractionModule).Assembly, serviceProvider);

        client.InteractionCreated += async (interaction) =>
        {
            var context = new SocketInteractionContext(client, interaction);
            await interactionService.ExecuteCommandAsync(context, serviceProvider);
        };

        await client.LoginAsync(TokenType.Bot, settings.BotToken);
        await client.StartAsync();

        client.Ready += async () =>
        {
            await interactionService.RegisterCommandsToGuildAsync(settings.GuildId);
        };

        var inviteUrl = $"https://discord.com/api/oauth2/authorize?client_id={settings.ClientId}&scope=applications.commands";
        logger.LogInformation("Discord bot is online. Invite URL: {InviteUrl}", inviteUrl);

        logger.LogInformation("Discord bot is online and listening.");

        await Task.Delay(Timeout.Infinite);
    }
}
