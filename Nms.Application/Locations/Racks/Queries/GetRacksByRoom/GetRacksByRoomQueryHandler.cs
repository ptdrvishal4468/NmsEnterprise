using MediatR;
using Nms.Application.Locations.Racks.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Locations.Racks.Queries.GetRacksByRoom;

public class GetRacksByRoomQueryHandler : IRequestHandler<GetRacksByRoomQuery, IReadOnlyList<RackDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetRacksByRoomQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<RackDto>> Handle(GetRacksByRoomQuery request, CancellationToken cancellationToken)
    {
        var racks = await _unitOfWork.Racks.GetByRoomIdAsync(request.RoomId, cancellationToken);

        return racks.Select(r => new RackDto(
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
            r.LastModifiedBy)).ToList();
    }
}