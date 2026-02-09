using System.Net.Http.Json;
using System.Text;

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

        StringBuilder sb = new();
        sb.Append(request.WebHookUrl);
        sb.Append("/reboot");
        var url = sb.ToString();
        
        HttpResponseMessage response = await Client.PostAsJsonAsync(url, body, cancellationToken);
        
        response.EnsureSuccessStatusCode();
        
        return $"Restarting the {request.ServerName} server.";
    }
}
