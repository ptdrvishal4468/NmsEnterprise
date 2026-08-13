using FluentValidation;

namespace Nms.Application.ConfigurationBackups.Commands.CreateBackupSchedule;

public class CreateBackupScheduleCommandValidator : AbstractValidator<CreateBackupScheduleCommand>
{
    public CreateBackupScheduleCommandValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage("Tenant ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Schedule name is required.")
            .MaximumLength(150).WithMessage("Schedule name cannot exceed 150 characters.");

        RuleFor(x => x.IntervalMinutes)
            .GreaterThanOrEqualTo(5).WithMessage("Backup schedule interval must be at least 5 minutes.");
    }
}