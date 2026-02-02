namespace DrPoro.Application.Commands.RestartServer;

public class RestartServerCommandHandler : IRequestHandler<RestartServerCommand, string>
{
    private readonly ILogger<RestartServerCommandHandler> _logger;

    public RestartServerCommandHandler(ILogger<RestartServerCommandHandler> logger)
    {
        _logger = logger;
    }

    public async Task<string> Handle(RestartServerCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Restarting the server: {ServerName}.", request.ServerName);
        
        return $"Restarting the {request.ServerName} server.";
    }
}