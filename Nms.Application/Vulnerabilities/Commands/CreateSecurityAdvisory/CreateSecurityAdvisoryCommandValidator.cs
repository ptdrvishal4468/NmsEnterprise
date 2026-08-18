using FluentValidation;

namespace Nms.Application.Vulnerabilities.Commands.CreateSecurityAdvisory;

public class CreateSecurityAdvisoryCommandValidator : AbstractValidator<CreateSecurityAdvisoryCommand>
{
    public CreateSecurityAdvisoryCommandValidator()
    {
        RuleFor(x => x.AdvisoryId)
            .NotEmpty().WithMessage("Advisory ID is required.")
            .MaximumLength(100).WithMessage("Advisory ID must not exceed 100 characters.");

        RuleFor(x => x.Vendor)
            .NotEmpty().WithMessage("Vendor is required.")
            .MaximumLength(100).WithMessage("Vendor must not exceed 100 characters.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(250).WithMessage("Title must not exceed 250 characters.");
    }
}