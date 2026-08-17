using MediatR;
using Nms.Application.Locations.Buildings.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Locations.Buildings.Commands.UpdateBuilding;

public class UpdateBuildingCommandHandler : IRequestHandler<UpdateBuildingCommand, BuildingDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBuildingCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<BuildingDto> Handle(UpdateBuildingCommand request, CancellationToken cancellationToken)
    {
        var building = await _unitOfWork.Buildings.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Building with ID '{request.Id}' was not found.");

        var dto = request.Dto;

        if (await _unitOfWork.Buildings.CodeExistsInSiteAsync(building.SiteId, dto.Code, request.Id, cancellationToken))
        {
            throw new InvalidOperationException($"Building code '{dto.Code}' already exists in this Site.");
        }

        building.Update(dto.Name, dto.Code, dto.Description, dto.Address);

        if (dto.IsActive)
        {
            building.Activate();
        }
        else
        {
            building.Deactivate();
        }

        _unitOfWork.Buildings.Update(building);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new BuildingDto(
            building.Id,
            building.TenantId,
            building.SiteId,
            building.Name,
            building.Code,
            building.Description,
            building.Address,
            building.IsActive,
            building.CreatedAtUtc,
            building.CreatedBy,
            building.LastModifiedAtUtc,
            building.LastModifiedBy,
            building.Floors != null ? building.Floors.Count : 0);
    }
}