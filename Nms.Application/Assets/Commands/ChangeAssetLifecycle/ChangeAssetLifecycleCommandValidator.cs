using FluentValidation;

namespace Nms.Application.Assets.Commands.ChangeAssetLifecycle;

public class ChangeAssetLifecycleCommandValidator : AbstractValidator<ChangeAssetLifecycleCommand>
{
    public ChangeAssetLifecycleCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Asset ID is required.");

        RuleFor(x => x.LifecycleState)
            .IsInEnum().WithMessage("A valid lifecycle state is required.");

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Lifecycle notes must not exceed 1000 characters.");
    }
}