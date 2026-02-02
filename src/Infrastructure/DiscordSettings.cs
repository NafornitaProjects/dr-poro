namespace DrPoro.Infrastructure;

public class DiscordSettings
{
    public string BotToken { get; set; } = string.Empty;
    
    public ulong ClientId { get; set; }
    
    public ulong GuildId { get; set; }

    public string WebHook { get; set; } = string.Empty;
}
