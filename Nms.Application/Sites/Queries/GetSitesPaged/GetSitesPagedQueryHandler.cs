using MediatR;
using Nms.Application.Common.Models;
using Nms.Application.Sites.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Sites.Queries.GetSitesPaged;

public class GetSitesPagedQueryHandler : IRequestHandler<GetSitesPagedQuery, PagedResult<SiteDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetSitesPagedQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<SiteDto>> Handle(GetSitesPagedQuery request, CancellationToken cancellationToken)
    {
        var allSites = await _unitOfWork.Sites.GetAllAsync(cancellationToken);
        var query = allSites.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.Trim().ToLowerInvariant();
            query = query.Where(s =>
                s.Name.ToLowerInvariant().Contains(term) ||
                s.Code.ToLowerInvariant().Contains(term) ||
                (s.City != null && s.City.ToLowerInvariant().Contains(term)) ||
                (s.Country != null && s.Country.ToLowerInvariant().Contains(term)));
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(s => s.IsActive == request.IsActive.Value);
        }

        var totalCount = query.Count();
        var items = query
            .OrderBy(s => s.Name)
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(s => new SiteDto(
                s.Id,
                s.TenantId,
                s.Name,
                s.Code,
                s.Description,
                s.Address,
                s.City,
                s.StateOrProvince,
                s.PostalCode,
                s.Country,
                s.Latitude,
                s.Longitude,
                s.TimeZone,
                s.IsActive,
                s.CreatedAtUtc,
                s.CreatedBy,
                s.LastModifiedAtUtc,
                s.LastModifiedBy,
                s.Buildings != null ? s.Buildings.Count : 0))
            .ToList();

        return new PagedResult<SiteDto>(items, totalCount, request.PageIndex, request.PageSize);
    }
}