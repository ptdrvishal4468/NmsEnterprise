using FluentValidation;

namespace Nms.Application.Firmware.Commands.UpdateUpgradePlanStatus;

public class UpdateUpgradePlanStatusCommandValidator : AbstractValidator<UpdateUpgradePlanStatusCommand>
{
    public UpdateUpgradePlanStatusCommandValidator()
    {
        RuleFor(x => x.PlanId)
            .NotEmpty().WithMessage("PlanId is required.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("A valid UpgradePlanStatus must be provided.");
    }
}