using MediatR;

namespace Nms.Application.Sites.Commands.DeleteSite;

public record DeleteSiteCommand(Guid Id) : IRequest<bool>;