using MediatR;
using Discord.Interactions;

using DrPoro.Application.Commands.UpdateClient;
using DrPoro.Infrastructure;

namespace DrPoro.Client.InteractionModules;

public class UpdateClientInteractionModule(IMediator mediator, IOptions<DiscordSettings> settings)
    : InteractionModuleBase<SocketInteractionContext>
{
    private readonly DiscordSettings _settings = settings.Value;

    [SlashCommand("update", "Updates a specified game client.")]
    public async Task UpdateClient([Summary("client-name", "The name of the client to update.")] string clientName)
    {
        try
        {
            await DeferAsync(ephemeral: true);

            await mediator.Send(new UpdateClientCommand(clientName, _settings.WebHook));

            await FollowupAsync($"Client {clientName} updated successfully.", ephemeral: true);
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
            Console.WriteLine($"Exception in UpdateCommand: {ex}");
        }
    }
}