using MediatR;

using Discord.Interactions;
using DrPoro.Application.Commands.RestartServer;

namespace DrPoro.Client.InteractionModules;

public class RestartInteractionModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IMediator _mediator;

    public RestartInteractionModule(IMediator mediator)
    {
        _mediator = mediator;
    }

    [SlashCommand("restart", "Restarts a named game server.")]
    public async Task RestartCommand([Summary("server-name", "The name of the server to restart.")] string serverName)
    {
        await RespondAsync();

        try
        {
            await _mediator.Send(new RestartServerCommand(serverName));
        }
        catch (FluentValidation.ValidationException validationEx)
        {
            var errors = string.Join("\n", validationEx.Errors.Select(e => $"{e.ErrorMessage}"));
        }
    }
}
