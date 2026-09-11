using MediatR;
using Nms.Application.Sites.Dtos;

namespace Nms.Application.Sites.Queries.GetSiteById;

public record GetSiteByIdQuery(Guid Id) : IRequest<SiteDto?>;