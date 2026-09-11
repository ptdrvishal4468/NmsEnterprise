using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Locations.Buildings.Dtos;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Application.Locations.Buildings.Commands.CreateBuilding;

public class CreateBuildingCommandHandler : IRequestHandler<CreateBuildingCommand, BuildingDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;

    public CreateBuildingCommandHandler(IUnitOfWork unitOfWork, ITenantContext tenantContext)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<BuildingDto> Handle(CreateBuildingCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var site = await _unitOfWork.Sites.GetByIdAsync(dto.SiteId, cancellationToken)
            ?? throw new KeyNotFoundException($"Site with ID '{dto.SiteId}' was not found.");

        if (await _unitOfWork.Buildings.CodeExistsInSiteAsync(dto.SiteId, dto.Code, null, cancellationToken))
        {
            throw new InvalidOperationException($"Building code '{dto.Code}' already exists in this Site.");
        }

        var building = new Building(
            Guid.NewGuid(),
            _tenantContext.TenantId,
            dto.SiteId,
            dto.Name,
            dto.Code,
            dto.Description,
            dto.Address);

        await _unitOfWork.Buildings.AddAsync(building, cancellationToken);
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
            0);
    }
}