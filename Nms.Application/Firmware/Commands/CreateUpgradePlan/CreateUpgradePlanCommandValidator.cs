using FluentValidation;

namespace Nms.Application.Firmware.Commands.CreateUpgradePlan;

public class CreateUpgradePlanCommandValidator : AbstractValidator<CreateUpgradePlanCommand>
{
    public CreateUpgradePlanCommandValidator()
    {
        RuleFor(x => x.DeviceId)
            .NotEmpty().WithMessage("DeviceId is required.");

        RuleFor(x => x.TargetVersion)
            .NotEmpty().WithMessage("Target version is required.")
            .MaximumLength(100).WithMessage("Target version must not exceed 100 characters.");

        RuleFor(x => x.PlannedDateUtc)
            .NotEmpty().WithMessage("Planned date is required.");

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Notes must not exceed 1000 characters.")
            .When(x => x.Notes != null);
    }
}