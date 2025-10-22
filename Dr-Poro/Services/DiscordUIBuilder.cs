namespace Dr_Poro.Services;

using Discord;

public interface IDiscordUIBuilder
{
    public ComponentBuilder CreateAvailabilityDropdown();
    public Modal CreateTimeModal(string day);
    public Embed CreateSetAvailabilityEmbed();
    public Embed CreateContinueSettingEmbed();
    public Embed CreateDayAddedEmbed(string day, string availabilityText, string currentAvailability);
    public MessageComponent CreateAvailabilityButtons();
    public Embed CreateFinalAvailabilityEmbed(string username, string userAvailability);
    public Embed CreateAllAvailabilitiesEmbed(string allAvailabilities, string username);

    public Embed CreateUserAvailabilityEmbed(string targetUsername, string userAvailability,
        string thumbnailUrl, string requestedByUsername);



}
public class DiscordUIBuilder: IDiscordUIBuilder
{
    public ComponentBuilder CreateAvailabilityDropdown()
    {
        SelectMenuBuilder selectMenu = new SelectMenuBuilder()
            .WithCustomId("select_day")
            .WithPlaceholder("Select a day to set availability")
            .AddOption("Monday", "Monday")
            .AddOption("Tuesday", "Tuesday")
            .AddOption("Wednesday", "Wednesday")
            .AddOption("Thursday", "Thursday")
            .AddOption("Friday", "Friday")
            .AddOption("Saturday", "Saturday")
            .AddOption("Sunday", "Sunday");

        return new ComponentBuilder().WithSelectMenu(selectMenu);
    }
    
    public Modal CreateTimeModal(string day)
    {
        return new ModalBuilder()
            .WithTitle($"{day}")
            .WithCustomId($"time_modal_{day}")
            .AddTextInput("Start Time", "start_time", TextInputStyle.Short,
                "e.g., 6:00 PM", required: true, value: "6:00 PM")
            .AddTextInput("End Time", "end_time", TextInputStyle.Short,
                "e.g., 11:59 PM", required: true, value: "11:59 PM")
            .Build();
    }
    
    public Embed CreateSetAvailabilityEmbed()
    {
        return new EmbedBuilder()
            .WithTitle("Set Your Availability")
            .WithDescription("Select a day from the dropdown below to set your availability.")
            .WithColor(Color.Blue)
            .Build();
    }
    
    public Embed CreateContinueSettingEmbed()
    {
        return new EmbedBuilder()
            .WithTitle("Set Your Availability")
            .WithDescription("Select another day from the dropdown below.")
            .WithColor(Color.Blue)
            .Build();
    }
    public Embed CreateDayAddedEmbed(string day, string availabilityText, string currentAvailability)
    {
        return new EmbedBuilder()
            .WithTitle("Day Added!")
            .WithDescription($"**{day}** set to: {availabilityText}\n\n" +
                           $"**Current Availability:**\n{currentAvailability}")
            .WithColor(Color.Green)
            .WithFooter("Click 'Add Another Day' to continue or 'Finish & Save' when done")
            .Build();
    }

    public MessageComponent CreateAvailabilityButtons()
    {
        return new ComponentBuilder()
            .WithButton("Add Another Day", "add_another_day", ButtonStyle.Primary, new Emoji("➕"))
            .WithButton("Finish & Save", "finish_availability", ButtonStyle.Success, new Emoji("✅"))
            .WithButton("Cancel", "cancel_availability", ButtonStyle.Danger, new Emoji("❌"))
            .Build();
    }

    public Embed CreateFinalAvailabilityEmbed(string username, string userAvailability)
    {
        return new EmbedBuilder()
            .WithTitle("Availability Saved!")
            .WithDescription($"**Your Weekly Availability:**\n{userAvailability}")
            .WithColor(Color.Green)
            .WithFooter($"Saved for {username}")
            .WithCurrentTimestamp()
            .Build();
    }

    public Embed CreateAllAvailabilitiesEmbed(string allAvailabilities, string username)
    {
        return new EmbedBuilder()
            .WithTitle("All User Availabilities")
            .WithDescription(allAvailabilities)
            .WithColor(Color.Blue)
            .WithFooter($"Requested by {username}")
            .WithCurrentTimestamp()
            .Build();
    }

    public Embed CreateUserAvailabilityEmbed(string targetUsername, string userAvailability, 
        string thumbnailUrl, string requestedByUsername)
    {
        return new EmbedBuilder()
            .WithTitle($"Availability for {targetUsername}")
            .WithDescription(userAvailability)
            .WithColor(Color.Green)
            .WithThumbnailUrl(thumbnailUrl)
            .WithFooter($"Requested by {requestedByUsername}")
            .WithCurrentTimestamp()
            .Build();
    }
}