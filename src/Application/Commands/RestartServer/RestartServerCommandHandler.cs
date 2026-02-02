namespace DrPoro.Application.Commands.RestartServer;

public class RestartServerCommandHandler(ILogger<RestartServerCommandHandler> logger)
    : IRequestHandler<RestartServerCommand, string>
{
    private static readonly HttpClient Client = new HttpClient();

    public async Task<string> Handle(RestartServerCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Restarting the server: {ServerName}.", request.ServerName);

        HttpResponseMessage response = await Client.PostAsync(request.WebHookUrl,  new StringContent(request.ServerName), cancellationToken);
        
        response.EnsureSuccessStatusCode();
        
        return $"Restarting the {request.ServerName} server.";
    }
}
