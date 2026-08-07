using FluentValidation;

namespace Nms.Application.Devices.Commands.ExecuteSshCommand;

public sealed class ExecuteSshCommandValidator : AbstractValidator<ExecuteSshCommand>
{
    public ExecuteSshCommandValidator()
    {
        RuleFor(x => x.DeviceId)
            .NotEmpty().WithMessage("Device ID is required.");

        RuleFor(x => x.Command)
            .NotEmpty().WithMessage("CLI command cannot be empty.")
            .MaximumLength(2000).WithMessage("CLI command cannot exceed 2000 characters.");

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("SSH Username is required.");

        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.Password) || !string.IsNullOrWhiteSpace(x.PrivateKey))
            .WithMessage("Either Password or PrivateKey must be provided for SSH authentication.");

        RuleFor(x => x.Port)
            .InclusiveBetween(1, 65535).WithMessage("Port must be between 1 and 65535.");

        RuleFor(x => x.TimeoutSeconds)
            .InclusiveBetween(1, 300).WithMessage("Timeout must be between 1 and 300 seconds.");
    }
}