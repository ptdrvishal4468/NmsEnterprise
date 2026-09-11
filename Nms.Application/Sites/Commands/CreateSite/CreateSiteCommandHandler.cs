using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Sites.Dtos;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Application.Sites.Commands.CreateSite;

public class CreateSiteCommandHandler : IRequestHandler<CreateSiteCommand, SiteDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;

    public CreateSiteCommandHandler(IUnitOfWork unitOfWork, ITenantContext tenantContext)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<SiteDto> Handle(CreateSiteCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        if (await _unitOfWork.Sites.CodeExistsAsync(dto.Code, null, cancellationToken))
        {
            throw new InvalidOperationException($"Site code '{dto.Code}' already exists for this tenant.");
        }

        var site = new Site(
            Guid.NewGuid(),
            _tenantContext.TenantId,
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

        await _unitOfWork.Sites.AddAsync(site, cancellationToken);
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
            0);
    }
}