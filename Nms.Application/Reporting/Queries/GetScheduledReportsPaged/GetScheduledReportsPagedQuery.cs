using MediatR;
using Nms.Application.Common.Models;
using Nms.Application.Reporting.Dtos;
using Nms.Domain.Enums;

namespace Nms.Application.Reporting.Queries.GetScheduledReportsPaged;

public record GetScheduledReportsPagedQuery(
    int PageIndex = 1,
    int PageSize = 10,
    ReportType? ReportType = null,
    bool? IsActive = null) : IRequest<PagedResult<ScheduledReportDto>>;