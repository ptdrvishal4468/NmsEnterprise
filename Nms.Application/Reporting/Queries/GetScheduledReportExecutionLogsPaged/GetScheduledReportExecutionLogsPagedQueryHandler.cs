using MediatR;
using Nms.Application.Common.Models;
using Nms.Application.Reporting.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Reporting.Queries.GetScheduledReportExecutionLogsPaged;

public class GetScheduledReportExecutionLogsPagedQueryHandler : IRequestHandler<GetScheduledReportExecutionLogsPagedQuery, PagedResult<ScheduledReportExecutionLogDto>>
{
    private readonly IScheduledReportExecutionLogRepository _executionLogRepository;

    public GetScheduledReportExecutionLogsPagedQueryHandler(IScheduledReportExecutionLogRepository executionLogRepository)
    {
        _executionLogRepository = executionLogRepository ?? throw new ArgumentNullException(nameof(executionLogRepository));
    }

    public async Task<PagedResult<ScheduledReportExecutionLogDto>> Handle(GetScheduledReportExecutionLogsPagedQuery request, CancellationToken cancellationToken)
    {
        var pageIndex = request.PageIndex < 1 ? 1 : request.PageIndex;
        var pageSize = request.PageSize < 1 ? 10 : (request.PageSize > 100 ? 100 : request.PageSize);

        var logs = await _executionLogRepository.FindAsync(
            l => l.ScheduledReportId == request.ScheduledReportId,
            cancellationToken);

        var totalCount = logs.Count;
        var items = logs
            .OrderByDescending(l => l.ExecutedAtUtc)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(l => new ScheduledReportExecutionLogDto
            {
                Id = l.Id,
                ScheduledReportId = l.ScheduledReportId,
                ExecutedAtUtc = l.ExecutedAtUtc,
                Status = l.Status,
                RecordCount = l.RecordCount,
                ErrorMessage = l.ErrorMessage
            })
            .ToList();

        return new PagedResult<ScheduledReportExecutionLogDto>(items, totalCount, pageIndex, pageSize);
    }
}