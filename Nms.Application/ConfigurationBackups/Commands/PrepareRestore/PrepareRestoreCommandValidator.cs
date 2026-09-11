using FluentValidation;

namespace Nms.Application.ConfigurationBackups.Commands.PrepareRestore;

public class PrepareRestoreCommandValidator : AbstractValidator<PrepareRestoreCommand>
{
    public PrepareRestoreCommandValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage("Tenant ID is required.");

        RuleFor(x => x.BackupId)
            .NotEmpty().WithMessage("Backup ID is required.");
    }
}