using MediatR;
using Nms.Application.Common.Models;
using Nms.Application.Locations.Rooms.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Locations.Rooms.Queries.GetRoomsPaged;

public class GetRoomsPagedQueryHandler : IRequestHandler<GetRoomsPagedQuery, PagedResult<RoomDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetRoomsPagedQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<RoomDto>> Handle(GetRoomsPagedQuery request, CancellationToken cancellationToken)
    {
        var allRooms = await _unitOfWork.Rooms.GetAllAsync(cancellationToken);
        var query = allRooms.AsQueryable();

        if (request.FloorId.HasValue)
        {
            query = query.Where(r => r.FloorId == request.FloorId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.Trim().ToLowerInvariant();
            query = query.Where(r =>
                r.Name.ToLowerInvariant().Contains(term) ||
                r.Code.ToLowerInvariant().Contains(term));
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(r => r.IsActive == request.IsActive.Value);
        }

        var totalCount = query.Count();
        var items = query
            .OrderBy(r => r.Name)
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(r => new RoomDto(
                r.Id,
                r.TenantId,
                r.FloorId,
                r.Name,
                r.Code,
                r.RoomType,
                r.Description,
                r.IsActive,
                r.CreatedAtUtc,
                r.CreatedBy,
                r.LastModifiedAtUtc,
                r.LastModifiedBy,
                r.Racks != null ? r.Racks.Count : 0))
            .ToList();

        return new PagedResult<RoomDto>(items, totalCount, request.PageIndex, request.PageSize);
    }
}