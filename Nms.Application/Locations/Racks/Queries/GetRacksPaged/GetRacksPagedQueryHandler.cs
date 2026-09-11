using MediatR;
using Nms.Application.Common.Models;
using Nms.Application.Locations.Racks.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Locations.Racks.Queries.GetRacksPaged;

public class GetRacksPagedQueryHandler : IRequestHandler<GetRacksPagedQuery, PagedResult<RackDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetRacksPagedQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<RackDto>> Handle(GetRacksPagedQuery request, CancellationToken cancellationToken)
    {
        var allRacks = await _unitOfWork.Racks.GetAllAsync(cancellationToken);
        var query = allRacks.AsQueryable();

        if (request.RoomId.HasValue)
        {
            query = query.Where(r => r.RoomId == request.RoomId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.Trim().ToLowerInvariant();
            query = query.Where(r =>
                r.Name.ToLowerInvariant().Contains(term) ||
                r.Identifier.ToLowerInvariant().Contains(term));
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(r => r.IsActive == request.IsActive.Value);
        }

        var totalCount = query.Count();
        var items = query
            .OrderBy(r => r.Identifier)
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(r => new RackDto(
                r.Id,
                r.TenantId,
                r.RoomId,
                r.Name,
                r.Identifier,
                r.HeightInUnits,
                r.WidthInInches,
                r.DepthInMm,
                r.MaxPowerWatts,
                r.MaxWeightKg,
                r.Notes,
                r.IsActive,
                r.CreatedAtUtc,
                r.CreatedBy,
                r.LastModifiedAtUtc,
                r.LastModifiedBy))
            .ToList();

        return new PagedResult<RackDto>(items, totalCount, request.PageIndex, request.PageSize);
    }
}