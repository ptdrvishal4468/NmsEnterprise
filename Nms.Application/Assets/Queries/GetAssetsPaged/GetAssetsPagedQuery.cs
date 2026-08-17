using MediatR;
using Nms.Application.Assets.Dtos;
using Nms.Application.Common.Models;
using Nms.Domain.Enums;

namespace Nms.Application.Assets.Queries.GetAssetsPaged;

public record GetAssetsPagedQuery(
    int PageNumber = 1,
    int PageSize = 20,
    string? SearchTerm = null,
    AssetLifecycleState? LifecycleState = null,
    WarrantyStatus? WarrantyStatus = null,
    string? Vendor = null,
    string? Department = null,
    string? SiteOrLocation = null) : IRequest<PagedResult<AssetDto>>;