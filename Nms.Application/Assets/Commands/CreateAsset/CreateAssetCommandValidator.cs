using FluentValidation;

namespace Nms.Application.Assets.Commands.CreateAsset;

public class CreateAssetCommandValidator : AbstractValidator<CreateAssetCommand>
{
    public CreateAssetCommandValidator()
    {
        RuleFor(x => x.AssetTag)
            .NotEmpty().WithMessage("Asset tag is required.")
            .MaximumLength(100).WithMessage("Asset tag must not exceed 100 characters.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Asset name is required.")
            .MaximumLength(200).WithMessage("Asset name must not exceed 200 characters.");

        RuleFor(x => x.LifecycleState)
            .IsInEnum().WithMessage("A valid lifecycle state is required.");

        RuleFor(x => x.WarrantyStatus)
            .IsInEnum().WithMessage("A valid warranty status is required.");

        RuleFor(x => x.PurchasePrice)
            .GreaterThanOrEqualTo(0).When(x => x.PurchasePrice.HasValue)
            .WithMessage("Purchase price cannot be negative.");

        RuleFor(x => x.SerialNumber).MaximumLength(100);
        RuleFor(x => x.Vendor).MaximumLength(100);
        RuleFor(x => x.Model).MaximumLength(100);
        RuleFor(x => x.Category).MaximumLength(100);
        RuleFor(x => x.SiteOrLocation).MaximumLength(200);
        RuleFor(x => x.RackIdentifier).MaximumLength(100);
        RuleFor(x => x.RackUnitPosition).MaximumLength(50);
        RuleFor(x => x.Department).MaximumLength(100);
        RuleFor(x => x.WarrantyProvider).MaximumLength(200);
        RuleFor(x => x.WarrantyContractNumber).MaximumLength(100);
        RuleFor(x => x.PurchaseOrderNumber).MaximumLength(100);
        RuleFor(x => x.Currency).MaximumLength(10);
    }
}