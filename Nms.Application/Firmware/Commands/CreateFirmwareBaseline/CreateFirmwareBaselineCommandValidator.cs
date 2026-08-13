using FluentValidation;

namespace Nms.Application.Firmware.Commands.CreateFirmwareBaseline;

public class CreateFirmwareBaselineCommandValidator : AbstractValidator<CreateFirmwareBaselineCommand>
{
    public CreateFirmwareBaselineCommandValidator()
    {
        RuleFor(x => x.Vendor)
            .NotEmpty().WithMessage("Vendor is required.")
            .MaximumLength(100).WithMessage("Vendor must not exceed 100 characters.");

        RuleFor(x => x.Model)
            .NotEmpty().WithMessage("Model is required.")
            .MaximumLength(100).WithMessage("Model must not exceed 100 characters.");

        RuleFor(x => x.TargetVersion)
            .NotEmpty().WithMessage("Target version is required.")
            .MaximumLength(100).WithMessage("Target version must not exceed 100 characters.");

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Notes must not exceed 1000 characters.")
            .When(x => x.Notes != null);
    }
}