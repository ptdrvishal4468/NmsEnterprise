using MediatR;
using Nms.Application.Assets.Dtos;
using Nms.Domain.Enums;

namespace Nms.Application.Assets.Commands.CreateAsset;

public record CreateAssetCommand(
    string AssetTag,
    string Name,
    AssetLifecycleState LifecycleState = AssetLifecycleState.Planned,
    string? SerialNumber = null,
    string? Vendor = null,
    string? Model = null,
    string? Category = null,
    string? SiteOrLocation = null,
    string? RackIdentifier = null,
    string? RackUnitPosition = null,
    string? Department = null,
    Guid? AssignedToUserId = null,
    Guid? DeviceId = null,
    string? WarrantyProvider = null,
    DateTime? WarrantyStartDateUtc = null,
    DateTime? WarrantyEndDateUtc = null,
    WarrantyStatus WarrantyStatus = WarrantyStatus.NotApplicable,
    string? WarrantyContractNumber = null,
    DateTime? PurchaseDateUtc = null,
    string? PurchaseOrderNumber = null,
    decimal? PurchasePrice = null,
    string? Currency = "USD") : IRequest<AssetDto>;