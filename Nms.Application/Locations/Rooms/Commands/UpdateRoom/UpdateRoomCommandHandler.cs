using MediatR;
using Nms.Application.Locations.Rooms.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Locations.Rooms.Commands.UpdateRoom;

public class UpdateRoomCommandHandler : IRequestHandler<UpdateRoomCommand, RoomDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateRoomCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<RoomDto> Handle(UpdateRoomCommand request, CancellationToken cancellationToken)
    {
        var room = await _unitOfWork.Rooms.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Room with ID '{request.Id}' was not found.");

        var dto = request.Dto;

        if (await _unitOfWork.Rooms.CodeExistsInFloorAsync(room.FloorId, dto.Code, request.Id, cancellationToken))
        {
            throw new InvalidOperationException($"Room code '{dto.Code}' already exists on this Floor.");
        }

        room.Update(dto.Name, dto.Code, dto.RoomType, dto.Description);

        if (dto.IsActive)
        {
            room.Activate();
        }
        else
        {
            room.Deactivate();
        }

        _unitOfWork.Rooms.Update(room);
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
            room.Racks != null ? room.Racks.Count : 0);
    }
}