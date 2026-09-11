using MediatR;
using Nms.Application.Locations.Racks.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Locations.Racks.Queries.GetRackById;

public class GetRackByIdQueryHandler : IRequestHandler<GetRackByIdQuery, RackDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetRackByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<RackDto?> Handle(GetRackByIdQuery request, CancellationToken cancellationToken)
    {
        var rack = await _unitOfWork.Racks.GetByIdAsync(request.Id, cancellationToken);
        if (rack == null)
        {
            return null;
        }

        return new RackDto(
            rack.Id,
            rack.TenantId,
            rack.RoomId,
            rack.Name,
            rack.Identifier,
            rack.HeightInUnits,
            rack.WidthInInches,
            rack.DepthInMm,
            rack.MaxPowerWatts,
            rack.MaxWeightKg,
            rack.Notes,
            rack.IsActive,
            rack.CreatedAtUtc,
            rack.CreatedBy,
            rack.LastModifiedAtUtc,
            rack.LastModifiedBy);
    }
}