using MediatR;
using Discord.Interactions;

using DrPoro.Application.Commands.RestartServer;
using DrPoro.Infrastructure;

namespace DrPoro.Client.InteractionModules;

public class RestartInteractionModule(IMediator mediator, IOptions<DiscordSettings> settings)
    : InteractionModuleBase<SocketInteractionContext>
{
    private readonly DiscordSettings _settings = settings.Value;

    [SlashCommand("restart", "Restarts a named game server.")]
    public async Task RestartCommand([Summary("server-name", "The name of the server to restart.")] string serverName)
    {
        try
        {
            await DeferAsync(ephemeral: true);

            await mediator.Send(new RestartServerCommand(serverName, _settings.WebHook));
            
            await FollowupAsync($"Server {serverName} restarted successfully.", ephemeral: true);
        }
        catch (FluentValidation.ValidationException validationEx)
        {
            var errors = string.Join("\n", validationEx.Errors.Select(e => $"{e.ErrorMessage}"));
            await FollowupAsync($"Validation failed:\n{errors}", ephemeral: true);
            Console.WriteLine(errors);
        }
        catch (Exception ex)
        {
            await FollowupAsync("An unexpected error occurred.", ephemeral: true);
            Console.WriteLine($"Exception in RestartCommand: {ex}");
        }
    }
}
