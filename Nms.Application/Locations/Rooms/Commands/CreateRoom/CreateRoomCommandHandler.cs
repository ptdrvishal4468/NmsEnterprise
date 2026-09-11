using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Locations.Rooms.Dtos;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Application.Locations.Rooms.Commands.CreateRoom;

public class CreateRoomCommandHandler : IRequestHandler<CreateRoomCommand, RoomDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;

    public CreateRoomCommandHandler(IUnitOfWork unitOfWork, ITenantContext tenantContext)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<RoomDto> Handle(CreateRoomCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var floor = await _unitOfWork.Floors.GetByIdAsync(dto.FloorId, cancellationToken)
            ?? throw new KeyNotFoundException($"Floor with ID '{dto.FloorId}' was not found.");

        if (await _unitOfWork.Rooms.CodeExistsInFloorAsync(dto.FloorId, dto.Code, null, cancellationToken))
        {
            throw new InvalidOperationException($"Room code '{dto.Code}' already exists on this Floor.");
        }

        var room = new Room(
            Guid.NewGuid(),
            _tenantContext.TenantId,
            dto.FloorId,
            dto.Name,
            dto.Code,
            dto.RoomType,
            dto.Description);

        await _unitOfWork.Rooms.AddAsync(room, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new RoomDto(
            room.Id,
            room.TenantId,
            room.FloorId,
            room.Name,
            room.Code,
            room.RoomType,
            room.Description,
            room.IsActive,
            room.CreatedAtUtc,
            room.CreatedBy,
            room.LastModifiedAtUtc,
            room.LastModifiedBy,
            0);
    }
}