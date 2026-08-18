using FluentValidation;

namespace Nms.Application.Cybersecurity.Commands.EvaluateDeviceCompliance;

public class EvaluateDeviceComplianceCommandValidator : AbstractValidator<EvaluateDeviceComplianceCommand>
{
    public EvaluateDeviceComplianceCommandValidator()
    {
        RuleFor(x => x.DeviceId)
            .NotEmpty().WithMessage("Device ID is required.");

        RuleFor(x => x.EvaluationNotes)
            .MaximumLength(1000).WithMessage("Evaluation notes cannot exceed 1000 characters.");
    }
}