using MediatR;
using Nms.Application.Common.Models;
using Nms.Application.Sites.Dtos;

namespace Nms.Application.Sites.Queries.GetSitesPaged;

public record GetSitesPagedQuery(
    int PageIndex = 1,
    int PageSize = 20,
    string? SearchTerm = null,
    bool? IsActive = null) : IRequest<PagedResult<SiteDto>>;