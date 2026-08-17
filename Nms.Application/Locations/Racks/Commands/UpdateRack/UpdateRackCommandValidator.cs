using FluentValidation;

namespace Nms.Application.Locations.Racks.Commands.UpdateRack;

public class UpdateRackCommandValidator : AbstractValidator<UpdateRackCommand>
{
    public UpdateRackCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Rack Id is required.");

        RuleFor(x => x.Dto.Name)
            .NotEmpty().WithMessage("Rack name is required.")
            .MaximumLength(150).WithMessage("Rack name must not exceed 150 characters.");

        RuleFor(x => x.Dto.Identifier)
            .NotEmpty().WithMessage("Rack identifier is required.")
            .MaximumLength(50).WithMessage("Rack identifier must not exceed 50 characters.");

        RuleFor(x => x.Dto.HeightInUnits)
            .GreaterThan(0).WithMessage("HeightInUnits must be greater than zero.");
    }
}