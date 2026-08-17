using MediatR;
using Nms.Application.Ticketing.Dtos;

namespace Nms.Application.Ticketing.Queries.GetTicketSyncLogs;

public record GetTicketSyncLogsQuery(Guid TicketId) : IRequest<IReadOnlyList<TicketSyncLogDto>>;