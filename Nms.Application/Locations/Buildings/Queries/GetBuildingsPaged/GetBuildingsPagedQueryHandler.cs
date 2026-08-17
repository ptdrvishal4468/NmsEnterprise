using MediatR;
using Nms.Application.Common.Models;
using Nms.Application.Locations.Buildings.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Locations.Buildings.Queries.GetBuildingsPaged;

public class GetBuildingsPagedQueryHandler : IRequestHandler<GetBuildingsPagedQuery, PagedResult<BuildingDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetBuildingsPagedQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<BuildingDto>> Handle(GetBuildingsPagedQuery request, CancellationToken cancellationToken)
    {
        var allBuildings = await _unitOfWork.Buildings.GetAllAsync(cancellationToken);
        var query = allBuildings.AsQueryable();

        if (request.SiteId.HasValue)
        {
            query = query.Where(b => b.SiteId == request.SiteId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.Trim().ToLowerInvariant();
            query = query.Where(b =>
                b.Name.ToLowerInvariant().Contains(term) ||
                b.Code.ToLowerInvariant().Contains(term));
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(b => b.IsActive == request.IsActive.Value);
        }

        var totalCount = query.Count();
        var items = query
            .OrderBy(b => b.Name)
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(b => new BuildingDto(
                b.Id,
                b.TenantId,
                b.SiteId,
                b.Name,
                b.Code,
                b.Description,
                b.Address,
                b.IsActive,
                b.CreatedAtUtc,
                b.CreatedBy,
                b.LastModifiedAtUtc,
                b.LastModifiedBy,
                b.Floors != null ? b.Floors.Count : 0))
            .ToList();

        return new PagedResult<BuildingDto>(items, totalCount, request.PageIndex, request.PageSize);
    }
}