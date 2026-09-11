using MediatR;
using Nms.Application.Assets.Dtos;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Application.Assets.Commands.ChangeAssetLifecycle;

public class ChangeAssetLifecycleCommandHandler : IRequestHandler<ChangeAssetLifecycleCommand, AssetDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public ChangeAssetLifecycleCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<AssetDto?> Handle(ChangeAssetLifecycleCommand request, CancellationToken cancellationToken)
    {
        var asset = await _unitOfWork.Assets.GetByIdAsync(request.Id, cancellationToken);
        if (asset == null)
        {
            return null;
        }

        asset.ChangeLifecycleState(request.LifecycleState, request.Notes);

        _unitOfWork.Assets.Update(asset);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(asset);
    }

    private static AssetDto MapToDto(Asset asset) => new()
    {
        Id = asset.Id,
        TenantId = asset.TenantId,
        AssetTag = asset.AssetTag,
        Name = asset.Name,
        SerialNumber = asset.SerialNumber,
        Vendor = asset.Vendor,
        Model = asset.Model,
        Category = asset.Category,
        LifecycleState = asset.LifecycleState,
        LifecycleNotes = asset.LifecycleNotes,
        LifecycleChangedAtUtc = asset.LifecycleChangedAtUtc,
        WarrantyProvider = asset.WarrantyProvider,
        WarrantyStartDateUtc = asset.WarrantyStartDateUtc,
        WarrantyEndDateUtc = asset.WarrantyEndDateUtc,
        WarrantyStatus = asset.WarrantyStatus,
        WarrantyContractNumber = asset.WarrantyContractNumber,
        PurchaseDateUtc = asset.PurchaseDateUtc,
        PurchaseOrderNumber = asset.PurchaseOrderNumber,
        PurchasePrice = asset.PurchasePrice,
        Currency = asset.Currency,
        SiteOrLocation = asset.SiteOrLocation,
        RackIdentifier = asset.RackIdentifier,
        RackUnitPosition = asset.RackUnitPosition,
        Department = asset.Department,
        AssignedToUserId = asset.AssignedToUserId,
        DeviceId = asset.DeviceId,
        CreatedAtUtc = asset.CreatedAtUtc,
        CreatedBy = asset.CreatedBy,
        LastModifiedAtUtc = asset.LastModifiedAtUtc,
        LastModifiedBy = asset.LastModifiedBy
    };
}