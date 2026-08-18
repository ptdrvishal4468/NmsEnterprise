using FluentValidation;

namespace Nms.Application.ThreatDetection.Commands.UpdateThreatIndicatorStatus;

public class UpdateThreatIndicatorStatusCommandValidator : AbstractValidator<UpdateThreatIndicatorStatusCommand>
{
    public UpdateThreatIndicatorStatusCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Indicator ID is required.");
        RuleFor(x => x.Status).IsInEnum().WithMessage("A valid threat status must be specified.");
    }
}