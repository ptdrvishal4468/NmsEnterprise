using MediatR;
using Nms.Application.Assets.Dtos;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Application.Assets.Commands.UpdateAsset;

public class UpdateAssetCommandHandler : IRequestHandler<UpdateAssetCommand, AssetDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAssetCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<AssetDto?> Handle(UpdateAssetCommand request, CancellationToken cancellationToken)
    {
        var asset = await _unitOfWork.Assets.GetByIdAsync(request.Id, cancellationToken);
        if (asset == null)
        {
            return null;
        }

        asset.UpdateDetails(
            name: request.Name,
            serialNumber: request.SerialNumber,
            vendor: request.Vendor,
            model: request.Model,
            category: request.Category,
            siteOrLocation: request.SiteOrLocation,
            rackIdentifier: request.RackIdentifier,
            rackUnitPosition: request.RackUnitPosition,
            department: request.Department,
            assignedToUserId: request.AssignedToUserId,
            deviceId: request.DeviceId,
            warrantyProvider: request.WarrantyProvider,
            warrantyStartDateUtc: request.WarrantyStartDateUtc,
            warrantyEndDateUtc: request.WarrantyEndDateUtc,
            warrantyStatus: request.WarrantyStatus,
            warrantyContractNumber: request.WarrantyContractNumber,
            purchaseDateUtc: request.PurchaseDateUtc,
            purchaseOrderNumber: request.PurchaseOrderNumber,
            purchasePrice: request.PurchasePrice,
            currency: request.Currency);

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