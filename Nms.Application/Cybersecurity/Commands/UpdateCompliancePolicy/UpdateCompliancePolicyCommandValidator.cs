using FluentValidation;

namespace Nms.Application.Cybersecurity.Commands.UpdateCompliancePolicy;

public class UpdateCompliancePolicyCommandValidator : AbstractValidator<UpdateCompliancePolicyCommand>
{
    public UpdateCompliancePolicyCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Policy ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Policy name is required.")
            .MaximumLength(200).WithMessage("Policy name cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");

        RuleFor(x => x.Category)
            .IsInEnum().WithMessage("Valid compliance category is required.");

        RuleFor(x => x.CheckType)
            .IsInEnum().WithMessage("Valid compliance check type is required.");

        RuleFor(x => x.Severity)
            .IsInEnum().WithMessage("Valid compliance severity is required.");
    }
}