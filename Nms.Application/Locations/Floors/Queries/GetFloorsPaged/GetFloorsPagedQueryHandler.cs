using MediatR;
using Nms.Application.Common.Models;
using Nms.Application.Locations.Floors.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Locations.Floors.Queries.GetFloorsPaged;

public class GetFloorsPagedQueryHandler : IRequestHandler<GetFloorsPagedQuery, PagedResult<FloorDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetFloorsPagedQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<FloorDto>> Handle(GetFloorsPagedQuery request, CancellationToken cancellationToken)
    {
        var allFloors = await _unitOfWork.Floors.GetAllAsync(cancellationToken);
        var query = allFloors.AsQueryable();

        if (request.BuildingId.HasValue)
        {
            query = query.Where(f => f.BuildingId == request.BuildingId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.Trim().ToLowerInvariant();
            query = query.Where(f => f.Name.ToLowerInvariant().Contains(term));
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(f => f.IsActive == request.IsActive.Value);
        }

        var totalCount = query.Count();
        var items = query
            .OrderBy(f => f.FloorNumber)
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(f => new FloorDto(
                f.Id,
                f.TenantId,
                f.BuildingId,
                f.Name,
                f.FloorNumber,
                f.Description,
                f.IsActive,
                f.CreatedAtUtc,
                f.CreatedBy,
                f.LastModifiedAtUtc,
                f.LastModifiedBy,
                f.Rooms != null ? f.Rooms.Count : 0))
            .ToList();

        return new PagedResult<FloorDto>(items, totalCount, request.PageIndex, request.PageSize);
    }
}