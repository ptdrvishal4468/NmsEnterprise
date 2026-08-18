using FluentValidation;

namespace Nms.Application.ThreatDetection.Commands.AnalyzeConfigurationDrift;

public class AnalyzeConfigurationDriftCommandValidator : AbstractValidator<AnalyzeConfigurationDriftCommand>
{
    public AnalyzeConfigurationDriftCommandValidator()
    {
        RuleFor(x => x.DeviceId)
            .Must(id => !id.HasValue || id.Value != Guid.Empty)
            .WithMessage("When provided, DeviceId must not be an empty GUID.");
    }
}