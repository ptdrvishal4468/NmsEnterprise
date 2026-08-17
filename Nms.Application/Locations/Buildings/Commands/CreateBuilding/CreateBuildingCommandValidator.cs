using FluentValidation;

namespace Nms.Application.Locations.Buildings.Commands.CreateBuilding;

public class CreateBuildingCommandValidator : AbstractValidator<CreateBuildingCommand>
{
    public CreateBuildingCommandValidator()
    {
        RuleFor(x => x.Dto.SiteId)
            .NotEmpty().WithMessage("SiteId is required.");

        RuleFor(x => x.Dto.Name)
            .NotEmpty().WithMessage("Building name is required.")
            .MaximumLength(200).WithMessage("Building name must not exceed 200 characters.");

        RuleFor(x => x.Dto.Code)
            .NotEmpty().WithMessage("Building code is required.")
            .MaximumLength(50).WithMessage("Building code must not exceed 50 characters.");

        RuleFor(x => x.Dto.Address)
            .MaximumLength(500).WithMessage("Address must not exceed 500 characters.");
    }
}