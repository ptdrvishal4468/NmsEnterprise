using MediatR;
using Nms.Application.Common.Models;
using Nms.Application.Reporting.Dtos;

namespace Nms.Application.Reporting.Queries.GetScheduledReportExecutionLogsPaged;

public record GetScheduledReportExecutionLogsPagedQuery(
    Guid ScheduledReportId,
    int PageIndex = 1,
    int PageSize = 10) : IRequest<PagedResult<ScheduledReportExecutionLogDto>>;