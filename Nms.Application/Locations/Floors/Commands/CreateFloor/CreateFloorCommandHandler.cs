using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Locations.Floors.Dtos;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Application.Locations.Floors.Commands.CreateFloor;

public class CreateFloorCommandHandler : IRequestHandler<CreateFloorCommand, FloorDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;

    public CreateFloorCommandHandler(IUnitOfWork unitOfWork, ITenantContext tenantContext)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<FloorDto> Handle(CreateFloorCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var building = await _unitOfWork.Buildings.GetByIdAsync(dto.BuildingId, cancellationToken)
            ?? throw new KeyNotFoundException($"Building with ID '{dto.BuildingId}' was not found.");

        if (await _unitOfWork.Floors.FloorNumberExistsInBuildingAsync(dto.BuildingId, dto.FloorNumber, null, cancellationToken))
        {
            throw new InvalidOperationException($"Floor number '{dto.FloorNumber}' already exists in this Building.");
        }

        var floor = new Floor(
            Guid.NewGuid(),
            _tenantContext.TenantId,
            dto.BuildingId,
            dto.Name,
            dto.FloorNumber,
            dto.Description);

        await _unitOfWork.Floors.AddAsync(floor, cancellationToken);
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
            0);
    }
}