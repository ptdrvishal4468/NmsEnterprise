using MediatR;
using Nms.Application.Sites.Dtos;

namespace Nms.Application.Sites.Commands.CreateSite;

public record CreateSiteCommand(CreateSiteDto Dto) : IRequest<SiteDto>;