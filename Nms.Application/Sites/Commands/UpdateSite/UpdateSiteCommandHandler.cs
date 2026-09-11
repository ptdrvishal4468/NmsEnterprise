using MediatR;
using Nms.Application.Sites.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Sites.Commands.UpdateSite;

public class UpdateSiteCommandHandler : IRequestHandler<UpdateSiteCommand, SiteDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSiteCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<SiteDto> Handle(UpdateSiteCommand request, CancellationToken cancellationToken)
    {
        var site = await _unitOfWork.Sites.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Site with ID '{request.Id}' was not found.");

        var dto = request.Dto;

        if (await _unitOfWork.Sites.CodeExistsAsync(dto.Code, request.Id, cancellationToken))
        {
            throw new InvalidOperationException($"Site code '{dto.Code}' is already taken.");
        }

        site.Update(
            dto.Name,
            dto.Code,
            dto.Description,
            dto.Address,
            dto.City,
            dto.StateOrProvince,
            dto.PostalCode,
            dto.Country,
            dto.Latitude,
            dto.Longitude,
            dto.TimeZone);

        if (dto.IsActive)
        {
            site.Activate();
        }
        else
        {
            site.Deactivate();
        }

        _unitOfWork.Sites.Update(site);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SiteDto(
            site.Id,
            site.TenantId,
            site.Name,
            site.Code,
            site.Description,
            site.Address,
            site.City,
            site.StateOrProvince,
            site.PostalCode,
            site.Country,
            site.Latitude,
            site.Longitude,
            site.TimeZone,
            site.IsActive,
            site.CreatedAtUtc,
            site.CreatedBy,
            site.LastModifiedAtUtc,
            site.LastModifiedBy,
            site.Buildings?.Count ?? 0);
    }
}