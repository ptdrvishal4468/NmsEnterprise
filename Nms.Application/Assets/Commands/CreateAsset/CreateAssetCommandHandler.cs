using MediatR;
using Nms.Application.Assets.Dtos;
using Nms.Application.Common.Interfaces;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Application.Assets.Commands.CreateAsset;

public class CreateAssetCommandHandler : IRequestHandler<CreateAssetCommand, AssetDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;

    public CreateAssetCommandHandler(IUnitOfWork unitOfWork, ITenantContext tenantContext)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<AssetDto> Handle(CreateAssetCommand request, CancellationToken cancellationToken)
    {
        var tagExists = await _unitOfWork.Assets.AssetTagExistsAsync(request.AssetTag, cancellationToken: cancellationToken);
        if (tagExists)
        {
            throw new InvalidOperationException($"An asset with tag '{request.AssetTag}' already exists.");
        }

        var asset = new Asset(
            id: Guid.NewGuid(),
            tenantId: _tenantContext.TenantId,
            assetTag: request.AssetTag,
            name: request.Name,
            lifecycleState: request.LifecycleState,
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

        await _unitOfWork.Assets.AddAsync(asset, cancellationToken);
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