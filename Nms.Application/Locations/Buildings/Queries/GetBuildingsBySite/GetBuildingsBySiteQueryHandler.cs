using MediatR;
using Nms.Application.Locations.Buildings.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Locations.Buildings.Queries.GetBuildingsBySite;

public class GetBuildingsBySiteQueryHandler : IRequestHandler<GetBuildingsBySiteQuery, IReadOnlyList<BuildingDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetBuildingsBySiteQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<BuildingDto>> Handle(GetBuildingsBySiteQuery request, CancellationToken cancellationToken)
    {
        var buildings = await _unitOfWork.Buildings.GetBySiteIdAsync(request.SiteId, cancellationToken);

        return buildings.Select(b => new BuildingDto(
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
            b.Floors != null ? b.Floors.Count : 0)).ToList();
    }
}