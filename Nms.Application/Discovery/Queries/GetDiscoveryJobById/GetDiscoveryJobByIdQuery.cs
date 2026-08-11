using MediatR;
using Nms.Application.Discovery.Dtos;

namespace Nms.Application.Discovery.Queries.GetDiscoveryJobById;

public record GetDiscoveryJobByIdQuery(Guid JobId) : IRequest<DiscoveryJobDto?>;