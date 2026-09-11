using FluentValidation;

namespace Nms.Application.Alerts.Commands.CreateAlertRule;

public class CreateAlertRuleCommandValidator : AbstractValidator<CreateAlertRuleCommand>
{
    public CreateAlertRuleCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Rule name is required.")
            .MaximumLength(200).WithMessage("Rule name must not exceed 200 characters.");

        RuleFor(x => x.MetricType)
            .IsInEnum().WithMessage("Invalid metric type.");

        RuleFor(x => x.Operator)
            .IsInEnum().WithMessage("Invalid comparison operator.");

        RuleFor(x => x.Severity)
            .IsInEnum().WithMessage("Invalid alert severity.");
    }
}