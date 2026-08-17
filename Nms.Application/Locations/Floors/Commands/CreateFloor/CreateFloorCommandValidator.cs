using FluentValidation;

namespace Nms.Application.Locations.Floors.Commands.CreateFloor;

public class CreateFloorCommandValidator : AbstractValidator<CreateFloorCommand>
{
    public CreateFloorCommandValidator()
    {
        RuleFor(x => x.Dto.BuildingId)
            .NotEmpty().WithMessage("BuildingId is required.");

        RuleFor(x => x.Dto.Name)
            .NotEmpty().WithMessage("Floor name is required.")
            .MaximumLength(100).WithMessage("Floor name must not exceed 100 characters.");
    }
}