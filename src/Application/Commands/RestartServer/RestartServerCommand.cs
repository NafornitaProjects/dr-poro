namespace DrPoro.Application.Commands.RestartServer;

/// <summary>
/// Command to restart a named game server.
/// </summary>
/// <param name="ServerName"></param>
/// <param name="WebHookUrl"></param>
public record RestartServerCommand(string ServerName, string WebHookUrl) : IRequest<string>;
