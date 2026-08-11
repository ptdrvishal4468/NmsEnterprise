using MediatR;
using Nms.Application.Interfaces.Dtos;

namespace Nms.Application.Interfaces.Queries.GetInterfaceHistory;

public record GetInterfaceHistoryQuery(Guid NetworkInterfaceId, DateTime StartUtc, DateTime EndUtc) : IRequest<IReadOnlyList<NetworkInterfaceHistoryDto>>;