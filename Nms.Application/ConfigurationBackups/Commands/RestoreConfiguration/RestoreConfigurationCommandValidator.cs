using FluentValidation;

namespace Nms.Application.ConfigurationBackups.Commands.RestoreConfiguration;

public class RestoreConfigurationCommandValidator : AbstractValidator<RestoreConfigurationCommand>
{
    public RestoreConfigurationCommandValidator()
    {
        RuleFor(x => x.BackupId)
            .NotEmpty()
            .WithMessage("Backup ID is required.");
    }
}