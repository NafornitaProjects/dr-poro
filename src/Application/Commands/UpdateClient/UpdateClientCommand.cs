namespace DrPoro.Application.Commands.UpdateClient;

/// <summary>
/// Command to update a specific server game client.
/// </summary>
/// <param name="ClientName"></param>
/// <param name="WebHookUrl"></param>
public record UpdateClientCommand(string ClientName, string WebHookUrl) : IRequest<string>;
