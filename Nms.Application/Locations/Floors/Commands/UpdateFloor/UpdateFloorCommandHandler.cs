using MediatR;
using Nms.Application.Locations.Floors.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Locations.Floors.Commands.UpdateFloor;

public class UpdateFloorCommandHandler : IRequestHandler<UpdateFloorCommand, FloorDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateFloorCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<FloorDto> Handle(UpdateFloorCommand request, CancellationToken cancellationToken)
    {
        var floor = await _unitOfWork.Floors.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Floor with ID '{request.Id}' was not found.");

        var dto = request.Dto;

        if (await _unitOfWork.Floors.FloorNumberExistsInBuildingAsync(floor.BuildingId, dto.FloorNumber, request.Id, cancellationToken))
        {
            throw new InvalidOperationException($"Floor number '{dto.FloorNumber}' already exists in this Building.");
        }

        floor.Update(dto.Name, dto.FloorNumber, dto.Description);

        if (dto.IsActive)
        {
            floor.Activate();
        }
        else
        {
            floor.Deactivate();
        }

        _unitOfWork.Floors.Update(floor);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new FloorDto(
            floor.Id,
            floor.TenantId,
            floor.BuildingId,
            floor.Name,
            floor.FloorNumber,
            floor.Description,
            floor.IsActive,
            floor.CreatedAtUtc,
            floor.CreatedBy,
            floor.LastModifiedAtUtc,
            floor.LastModifiedBy,
            floor.Rooms != null ? floor.Rooms.Count : 0);
    }
}