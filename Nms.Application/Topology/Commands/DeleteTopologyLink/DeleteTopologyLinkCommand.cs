using MediatR;

namespace Nms.Application.Topology.Commands.DeleteTopologyLink;

public record DeleteTopologyLinkCommand(Guid Id) : IRequest<bool>;