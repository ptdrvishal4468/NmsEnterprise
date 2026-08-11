using FluentValidation;

namespace Nms.Application.Alerts.Commands.UpdateAlertRule;

public class UpdateAlertRuleCommandValidator : AbstractValidator<UpdateAlertRuleCommand>
{
    public UpdateAlertRuleCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Rule ID is required.");
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.MetricType).IsInEnum();
        RuleFor(x => x.Operator).IsInEnum();
        RuleFor(x => x.Severity).IsInEnum();
    }
}