using MediatR;
using Nms.Application.Locations.Racks.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Locations.Racks.Commands.UpdateRack;

public class UpdateRackCommandHandler : IRequestHandler<UpdateRackCommand, RackDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateRackCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<RackDto> Handle(UpdateRackCommand request, CancellationToken cancellationToken)
    {
        var rack = await _unitOfWork.Racks.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Rack with ID '{request.Id}' was not found.");

        var dto = request.Dto;

        if (await _unitOfWork.Racks.IdentifierExistsInRoomAsync(rack.RoomId, dto.Identifier, request.Id, cancellationToken))
        {
            throw new InvalidOperationException($"Rack identifier '{dto.Identifier}' already exists in this Room.");
        }

        rack.Update(
            dto.Name,
            dto.Identifier,
            dto.HeightInUnits,
            dto.WidthInInches,
            dto.DepthInMm,
            dto.MaxPowerWatts,
            dto.MaxWeightKg,
            dto.Notes);

        if (dto.IsActive)
        {
            rack.Activate();
        }
        else
        {
            rack.Deactivate();
        }

        _unitOfWork.Racks.Update(rack);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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