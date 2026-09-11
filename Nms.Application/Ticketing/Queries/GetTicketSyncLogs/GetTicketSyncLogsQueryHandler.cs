using MediatR;
using Nms.Application.Ticketing.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Ticketing.Queries.GetTicketSyncLogs;

public class GetTicketSyncLogsQueryHandler : IRequestHandler<GetTicketSyncLogsQuery, IReadOnlyList<TicketSyncLogDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTicketSyncLogsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<TicketSyncLogDto>> Handle(GetTicketSyncLogsQuery request, CancellationToken cancellationToken)
    {
        var logs = await _unitOfWork.TicketSyncLogs.GetByTicketIdAsync(request.TicketId, cancellationToken);

        return logs.Select(l => new TicketSyncLogDto(
            l.Id,
            l.TicketId,
            l.ProviderType,
            l.Direction,
            l.Status,
            l.RequestPayload,
            l.ResponsePayload,
            l.ErrorMessage,
            l.TimestampUtc)).ToList();
    }
}