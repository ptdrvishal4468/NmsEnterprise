using MediatR;
using Nms.Application.Sites.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Sites.Queries.GetSiteById;

public class GetSiteByIdQueryHandler : IRequestHandler<GetSiteByIdQuery, SiteDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetSiteByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<SiteDto?> Handle(GetSiteByIdQuery request, CancellationToken cancellationToken)
    {
        var site = await _unitOfWork.Sites.GetWithHierarchyAsync(request.Id, cancellationToken);
        if (site == null)
        {
            return null;
        }

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