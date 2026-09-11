using MediatR;
using Nms.Application.Sites.Dtos;

namespace Nms.Application.Sites.Commands.UpdateSite;

public record UpdateSiteCommand(Guid Id, UpdateSiteDto Dto) : IRequest<SiteDto>;