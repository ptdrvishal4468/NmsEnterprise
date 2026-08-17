using FluentValidation;

namespace Nms.Application.Sites.Commands.UpdateSite;

public class UpdateSiteCommandValidator : AbstractValidator<UpdateSiteCommand>
{
    public UpdateSiteCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Site Id is required.");

        RuleFor(x => x.Dto.Name)
            .NotEmpty().WithMessage("Site name is required.")
            .MaximumLength(200).WithMessage("Site name must not exceed 200 characters.");

        RuleFor(x => x.Dto.Code)
            .NotEmpty().WithMessage("Site code is required.")
            .MaximumLength(50).WithMessage("Site code must not exceed 50 characters.");

        RuleFor(x => x.Dto.Country)
            .MaximumLength(100).WithMessage("Country must not exceed 100 characters.");

        RuleFor(x => x.Dto.TimeZone)
            .MaximumLength(100).WithMessage("TimeZone must not exceed 100 characters.");
    }
}