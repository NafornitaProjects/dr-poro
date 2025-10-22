using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Dr_Poro.Services;

class Program
{
    public static async Task Main(string[] args)
    {
        await Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
                {
                    GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
                }));
                services.AddHostedService<DiscordBot>();
                services.AddSingleton<SlashCommandHandler>();
                services.AddSingleton<IAvailability, Availability>();
                services.AddHostedService<WeeklyAvailabilityClear>();
                services.AddSingleton<IPythonInterop, PythonInterop>();
                services.AddSingleton<ISlashCommandRegistry, SlashCommandRegistry>();
                services.AddSingleton<IDiscordUIBuilder, DiscordUIBuilder>();
                services.AddSingleton<IAvailabilityWorkflow, AvailabilityWorkflow>();
                services.AddSingleton<IAvailabilityRepository>(sp => 
                    new AvailabilityRepository(context.Configuration["FilePaths:AvailabilityFile"]));
            })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            })
            .RunConsoleAsync();
    }
}