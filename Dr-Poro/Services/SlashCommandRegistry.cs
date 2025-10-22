using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace Dr_Poro.Services;

public interface ISlashCommandRegistry
{
    Task RegisterCommandsAsync();
}

public class SlashCommandRegistry : ISlashCommandRegistry
{
    private readonly DiscordSocketClient _client;
    private readonly ulong _guildId;
    
    public SlashCommandRegistry(DiscordSocketClient client, IConfiguration configuration)
    {
        _guildId = ulong.Parse(configuration["GuildId"]);
        _client = client;
    }
    public async Task RegisterCommandsAsync()
    {
        SocketGuild guild = _client.GetGuild(_guildId);

        SlashCommandProperties setAvailabilityCommand = RegisterSetAvailabilityCommand();
        SlashCommandProperties getAvailabilityCommand = RegisterGetAvailabilityCommand();
        
        await guild.CreateApplicationCommandAsync(setAvailabilityCommand);
        await guild.CreateApplicationCommandAsync(getAvailabilityCommand);
    }

    private SlashCommandProperties RegisterSetAvailabilityCommand()
    {
        SlashCommandProperties setAvailabilityCommand = new SlashCommandBuilder()
            .WithName("set-availability")
            .WithDescription("Set your availability for this week")
            .Build();

        return setAvailabilityCommand;
    }

    private SlashCommandProperties RegisterGetAvailabilityCommand()
    {
        SlashCommandProperties getAvailabilityCommand = new SlashCommandBuilder()
            .WithName("get-availability")
            .WithDescription("Get your availability for this week")
            .AddOption(new SlashCommandOptionBuilder()
                .WithName("all")
                .WithDescription("Get all availability for this week")
                .WithType(ApplicationCommandOptionType.SubCommand))
            .AddOption(new SlashCommandOptionBuilder()
                .WithName("user")
                .WithDescription("Get user's availability for this week")
                .WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption("target", ApplicationCommandOptionType.User,
                    "The user to get availability for", isRequired: true))
            .Build();

        return getAvailabilityCommand;
    }
}