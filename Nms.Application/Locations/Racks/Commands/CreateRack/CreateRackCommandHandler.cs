using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Locations.Racks.Dtos;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Application.Locations.Racks.Commands.CreateRack;

public class CreateRackCommandHandler : IRequestHandler<CreateRackCommand, RackDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;

    public CreateRackCommandHandler(IUnitOfWork unitOfWork, ITenantContext tenantContext)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<RackDto> Handle(CreateRackCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var room = await _unitOfWork.Rooms.GetByIdAsync(dto.RoomId, cancellationToken)
            ?? throw new KeyNotFoundException($"Room with ID '{dto.RoomId}' was not found.");

        if (await _unitOfWork.Racks.IdentifierExistsInRoomAsync(dto.RoomId, dto.Identifier, null, cancellationToken))
        {
            throw new InvalidOperationException($"Rack identifier '{dto.Identifier}' already exists in this Room.");
        }

        var rack = new Rack(
            Guid.NewGuid(),
            _tenantContext.TenantId,
            dto.RoomId,
            dto.Name,
            dto.Identifier,
            dto.HeightInUnits,
            dto.WidthInInches,
            dto.DepthInMm,
            dto.MaxPowerWatts,
            dto.MaxWeightKg,
            dto.Notes);

        await _unitOfWork.Racks.AddAsync(rack, cancellationToken);
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