using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace Dr_Poro.Services;

public class SlashCommandHandler
{
    private readonly DiscordSocketClient _client;
    private readonly IAvailability _availabilityService;
    private readonly IPythonInterop _pythonService;
    private readonly IDiscordUIBuilder _uiBuilder;
    private readonly IAvailabilityWorkflow _workflow;
    private readonly IAvailabilityRepository _repository;
    private readonly string? _availabilityFilePath;

    public SlashCommandHandler(
        DiscordSocketClient client,
        IAvailability availabilityService,
        IConfiguration configuration,
        IPythonInterop pythonService)
    {
        _client = client;
        _availabilityService = availabilityService;
        _pythonService = pythonService;
        _availabilityFilePath = configuration["FilePaths:AvailabilityFile"];

        _uiBuilder = new DiscordUIBuilder();
        _workflow = new AvailabilityWorkflow();
        _repository = new AvailabilityRepository(_availabilityFilePath);

        _client.SelectMenuExecuted += HandleSelectMenuAsync;
        _client.ButtonExecuted += HandleButtonAsync;
        _client.ModalSubmitted += HandleModalAsync;
    }

    private async Task HandleSetAvailabilitiesCommandAsync(SocketSlashCommand command)
    {
        await command.DeferAsync(ephemeral: true);
        _workflow.InitializeSession(command.User.Id);
        await command.FollowupAsync(embed: _uiBuilder.CreateSetAvailabilityEmbed(),
            components: _uiBuilder.CreateAvailabilityDropdown().Build(), ephemeral: true);
    }

    private async Task HandleSelectMenuAsync(SocketMessageComponent component)
    {
        if (component.Data.CustomId != "select_day")
            return;

        string selectedDay = component.Data.Values.First();
        Modal modal = _uiBuilder.CreateTimeModal(selectedDay);

        try
        {
            await component.RespondWithModalAsync(modal);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending modal: {ex.Message}");
        }
    }

    private async Task HandleModalAsync(SocketModal modal)
    {
        if (!modal.Data.CustomId.StartsWith("time_modal_"))
            return;

        await modal.DeferAsync(ephemeral: true);

        List<SocketMessageComponentData> components = modal.Data.Components.ToList();
        string day = modal.Data.CustomId.Replace("time_modal_", "");
        string startTime = components.FirstOrDefault(c => c.CustomId == "start_time")?.Value ?? "";
        string endTime = components.FirstOrDefault(c => c.CustomId == "end_time")?.Value ?? "";
        string availabilityText = $"{startTime} - {endTime}";

        _workflow.SetDayAvailability(modal.User.Id, day, availabilityText);

        string currentAvailability = _workflow.FormatCurrentAvailability(modal.User.Id);
        Embed embed = _uiBuilder.CreateDayAddedEmbed(day, availabilityText, currentAvailability);

        await modal.FollowupAsync(embed: embed, components: _uiBuilder.CreateAvailabilityButtons(), ephemeral: true);
    }

    private async Task HandleButtonAsync(SocketMessageComponent component)
    {
        switch (component.Data.CustomId)
        {
            case "add_another_day":
                await component.DeferAsync(ephemeral: true);
                await component.FollowupAsync(embed: _uiBuilder.CreateContinueSettingEmbed(),
                    components: _uiBuilder.CreateAvailabilityDropdown().Build(), ephemeral: true);
                break;

            case "finish_availability":
                await component.DeferAsync(ephemeral: true);

                if (!_workflow.HasSessionData(component.User.Id))
                {
                    await component.FollowupAsync("No availability data to save!", ephemeral: true);
                    return;
                }

                var sessionData = _workflow.GetSessionData(component.User.Id);
                await _repository.SaveUserAvailabilityAsync(component.User.Username, sessionData);

                string userAvailability = await _availabilityService.GetAvailabilityTextAsync(component.User.Username);
                Embed finalEmbed = _uiBuilder.CreateFinalAvailabilityEmbed(component.User.Username, userAvailability);

                await component.FollowupAsync(embed: finalEmbed, ephemeral: true);
                _workflow.ClearSession(component.User.Id);

                await component.ModifyOriginalResponseAsync(msg => msg.Components = new ComponentBuilder().Build());
                break;

            case "cancel_availability":
                await component.DeferAsync(ephemeral: true);
                _workflow.ClearSession(component.User.Id);
                await component.FollowupAsync("Availability setting cancelled.", ephemeral: true);
                await component.ModifyOriginalResponseAsync(msg => msg.Components = new ComponentBuilder().Build());
                break;
        }
    }

    private async Task HandleGetAvailabilitiesCommandAsync(SocketSlashCommand command)
    {
        await command.DeferAsync(ephemeral: true);
        SocketSlashCommandDataOption subcommand = command.Data.Options.FirstOrDefault();

        if (subcommand?.Name == "all")
        {
            string allAvailabilities = await _availabilityService.GetAllAvailabilitiesTextAsync();

            if (string.IsNullOrEmpty(allAvailabilities))
            {
                await command.FollowupAsync("No availabilities have been set yet!", ephemeral: true);
                return;
            }

            string? imagePath = null;

            if (!string.IsNullOrEmpty(_availabilityFilePath) && File.Exists(_availabilityFilePath))
            {
                try
                {
                    string availabilityDataJson = await File.ReadAllTextAsync(_availabilityFilePath);
                    imagePath = await _pythonService.GenerateAvailabilityImageAsync(availabilityDataJson);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error generating availability image: {ex.Message}");
                }
            }

            EmbedBuilder embedBuilder = new EmbedBuilder()
                .WithTitle("All User Availabilities")
                .WithDescription(allAvailabilities)
                .WithColor(Color.Blue)
                .WithFooter($"Requested by {command.User.Username}")
                .WithCurrentTimestamp();

            if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
            {
                embedBuilder.WithImageUrl("attachment://availability.png");
                await command.FollowupWithFileAsync(filePath: imagePath, embed: embedBuilder.Build(),
                    fileName: "availability.png", ephemeral: true);
            }
            else
            {
                await command.FollowupAsync(embed: embedBuilder.Build(), ephemeral: true);
            }
        }
        else if (subcommand?.Name == "user")
        {
            SocketUser targetUser = subcommand.Options.FirstOrDefault(o => o.Name == "target")?.Value as SocketUser;

            if (targetUser == null)
            {
                await command.FollowupAsync("Could not find the specified user!", ephemeral: true);
                return;
            }

            string userAvailability = await _availabilityService.GetAvailabilityTextAsync(targetUser.Username);

            if (string.IsNullOrEmpty(userAvailability))
            {
                await command.FollowupAsync($"{targetUser.Mention} has not set their availability yet!", ephemeral: true);
                return;
            }

            Embed embed = _uiBuilder.CreateUserAvailabilityEmbed(targetUser.Username, userAvailability,
                targetUser.GetAvatarUrl() ?? targetUser.GetDefaultAvatarUrl(), command.User.Username);

            await command.FollowupAsync(embed: embed, ephemeral: true);
        }
        else
        {
            await command.FollowupAsync("Please specify either 'all' or 'user' subcommand!", ephemeral: true);
        }
    }

    public async Task HandleCommandAsync(SocketSlashCommand command)
    {
        switch (command.CommandName)
        {
            case "set-availability":
                await HandleSetAvailabilitiesCommandAsync(command);
                break;
            case "get-availability":
                await HandleGetAvailabilitiesCommandAsync(command);
                break;
            default:
                string errorMessage = $"Sorry, but I do not understand {command.CommandName}.";
                await command.RespondAsync(errorMessage, ephemeral: true);
                break;
        }
    }
}