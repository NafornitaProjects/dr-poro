using System.Net.Http.Json;

namespace DrPoro.Application.Commands.RestartServer;

public class Server
{
    public string ServerName { get; set; } = string.Empty;
}

public class RestartServerCommandHandler(ILogger<RestartServerCommandHandler> logger)
    : IRequestHandler<RestartServerCommand, string>
{
    private static readonly HttpClient Client = new();

    public async Task<string> Handle(RestartServerCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Restarting the server: {ServerName}.", request.ServerName);

        var body = new Server
        {
            ServerName = request.ServerName
        };

        Client.DefaultRequestHeaders.ExpectContinue = false;
        
        HttpResponseMessage response = await Client.PostAsJsonAsync(request.WebHookUrl, body, cancellationToken);
        
        response.EnsureSuccessStatusCode();
        
        return $"Restarting the {request.ServerName} server.";
    }
}
