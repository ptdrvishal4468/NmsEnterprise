using FluentValidation;

namespace Nms.Application.ThreatDetection.Commands.CreateThreatRule;

public class CreateThreatRuleCommandValidator : AbstractValidator<CreateThreatRuleCommand>
{
    public CreateThreatRuleCommandValidator()
    {
        RuleFor(x => x.RuleName)
            .NotEmpty().WithMessage("Rule name is required.")
            .MaximumLength(150).WithMessage("Rule name cannot exceed 150 characters.");

        RuleFor(x => x.FailureThreshold)
            .GreaterThan(0).WithMessage("Failure threshold must be greater than zero.");

        RuleFor(x => x.TimeWindowMinutes)
            .GreaterThan(0).WithMessage("Time window must be greater than zero.");
    }
}