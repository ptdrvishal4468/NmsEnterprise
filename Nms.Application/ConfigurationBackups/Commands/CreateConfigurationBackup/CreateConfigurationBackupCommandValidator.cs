using FluentValidation;

namespace Nms.Application.ConfigurationBackups.Commands.CreateConfigurationBackup;

public class CreateConfigurationBackupCommandValidator : AbstractValidator<CreateConfigurationBackupCommand>
{
    public CreateConfigurationBackupCommandValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage("Tenant ID is required.");

        RuleFor(x => x.DeviceId)
            .NotEmpty().WithMessage("Device ID is required.");
    }
}