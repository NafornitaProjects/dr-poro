namespace DrPoro.Application.Commands.RestartServer;

/// <summary>
/// Command to restart a named game server.
/// </summary>
/// <param name="ServerName"></param>
public record RestartServerCommand(string ServerName) : IRequest<string>;
