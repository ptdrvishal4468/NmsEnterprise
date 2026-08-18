using FluentValidation;

namespace Nms.Application.Vulnerabilities.Commands.AnalyzeDeviceVulnerabilities;

public class AnalyzeDeviceVulnerabilitiesCommandValidator : AbstractValidator<AnalyzeDeviceVulnerabilitiesCommand>
{
    public AnalyzeDeviceVulnerabilitiesCommandValidator()
    {
        RuleFor(x => x.DeviceId)
            .NotEmpty().WithMessage("Device ID is required.");
    }
}