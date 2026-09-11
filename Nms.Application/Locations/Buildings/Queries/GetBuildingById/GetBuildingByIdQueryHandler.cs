using MediatR;
using Nms.Application.Locations.Buildings.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Locations.Buildings.Queries.GetBuildingById;

public class GetBuildingByIdQueryHandler : IRequestHandler<GetBuildingByIdQuery, BuildingDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetBuildingByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<BuildingDto?> Handle(GetBuildingByIdQuery request, CancellationToken cancellationToken)
    {
        var building = await _unitOfWork.Buildings.GetByIdAsync(request.Id, cancellationToken);
        if (building == null)
        {
            return null;
        }

        return new BuildingDto(
            building.Id,
            building.TenantId,
            building.SiteId,
            building.Name,
            building.Code,
            building.Description,
            building.Address,
            building.IsActive,
            building.CreatedAtUtc,
            building.CreatedBy,
            building.LastModifiedAtUtc,
            building.LastModifiedBy,
            building.Floors != null ? building.Floors.Count : 0);
    }
}