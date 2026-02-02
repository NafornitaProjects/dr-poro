using FluentValidation;

namespace DrPoro.Application.Commands.RestartServer;

public class RestartServerCommandValidator : AbstractValidator<RestartServerCommand>
{
    public RestartServerCommandValidator()
    {
        RuleFor(x => x.ServerName)
            .NotEmpty()
            .WithMessage("Server name must not be empty.")
            .MaximumLength(64)
            .WithMessage("Server name must not exceed 64 characters.");
    }
}
