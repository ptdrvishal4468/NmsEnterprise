using MediatR;
using Nms.Application.Assets.Dtos;
using Nms.Application.Common.Models;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Application.Assets.Queries.GetAssetsPaged;

public class GetAssetsPagedQueryHandler : IRequestHandler<GetAssetsPagedQuery, PagedResult<AssetDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAssetsPagedQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<AssetDto>> Handle(GetAssetsPagedQuery request, CancellationToken cancellationToken)
    {
        var allAssets = await _unitOfWork.Assets.GetAllAsync(cancellationToken);

        var query = allAssets.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.Trim().ToLowerInvariant();
            query = query.Where(a =>
                a.AssetTag.ToLowerInvariant().Contains(term) ||
                a.Name.ToLowerInvariant().Contains(term) ||
                (!string.IsNullOrEmpty(a.SerialNumber) && a.SerialNumber.ToLowerInvariant().Contains(term)));
        }

        if (request.LifecycleState.HasValue)
        {
            query = query.Where(a => a.LifecycleState == request.LifecycleState.Value);
        }

        if (request.WarrantyStatus.HasValue)
        {
            query = query.Where(a => a.WarrantyStatus == request.WarrantyStatus.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Vendor))
        {
            query = query.Where(a => string.Equals(a.Vendor, request.Vendor, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Department))
        {
            query = query.Where(a => string.Equals(a.Department, request.Department, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.SiteOrLocation))
        {
            query = query.Where(a => string.Equals(a.SiteOrLocation, request.SiteOrLocation, StringComparison.OrdinalIgnoreCase));
        }

        var assetList = query.OrderByDescending(a => a.CreatedAtUtc).ToList();
        var totalCount = assetList.Count;

        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 20 : (request.PageSize > 100 ? 100 : request.PageSize);

        var pagedItems = assetList
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(MapToDto)
            .ToList();

        return new PagedResult<AssetDto>(pagedItems, totalCount, pageNumber, pageSize);
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