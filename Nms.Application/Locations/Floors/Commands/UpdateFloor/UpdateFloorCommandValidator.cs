using FluentValidation;

namespace Nms.Application.Locations.Floors.Commands.UpdateFloor;

public class UpdateFloorCommandValidator : AbstractValidator<UpdateFloorCommand>
{
    public UpdateFloorCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Floor Id is required.");

        RuleFor(x => x.Dto.Name)
            .NotEmpty().WithMessage("Floor name is required.")
            .MaximumLength(100).WithMessage("Floor name must not exceed 100 characters.");
    }
}