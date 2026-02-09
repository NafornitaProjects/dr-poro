using System.Net.Http.Json;

namespace DrPoro.Application.Commands.UpdateClient;

public class Client
{
    public string ClientName { get; set; } = string.Empty;
}

public class UpdateClientCommandHandler(ILogger<UpdateClientCommandHandler> logger)
    : IRequestHandler<UpdateClientCommand, string>
{
    private static readonly HttpClient Client = new();

    public async Task<string> Handle(UpdateClientCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating the following server game client: {ClientName}.", request.ClientName);

        var body = new Client
        {
            ClientName = request.ClientName
        };

        Client.DefaultRequestHeaders.ExpectContinue = false;
        
        HttpResponseMessage response = await Client.PostAsJsonAsync(request.WebHookUrl, body, cancellationToken);
        
        response.EnsureSuccessStatusCode();
        
        return $"Updating the {request.ClientName} server game client.";
    }
}
