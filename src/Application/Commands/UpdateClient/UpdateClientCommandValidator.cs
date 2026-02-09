using FluentValidation;

namespace DrPoro.Application.Commands.UpdateClient;

public class UpdateClientCommandValidator : AbstractValidator<UpdateClientCommand>
{
    public UpdateClientCommandValidator()
    {
        RuleFor(x => x.ClientName)
            .NotEmpty()
            .WithMessage("Client name must not be empty.")
            .MaximumLength(64)
            .WithMessage("Client name must not exceed 64 characters.");

        RuleFor(x => x.WebHookUrl)
            .NotEmpty()
            .WithMessage("WebHook endpoints must not be empty.");
    }
}
