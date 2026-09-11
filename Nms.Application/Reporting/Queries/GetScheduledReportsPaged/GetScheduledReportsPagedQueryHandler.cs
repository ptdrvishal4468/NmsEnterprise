using MediatR;
using Nms.Application.Common.Models;
using Nms.Application.Reporting.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Reporting.Queries.GetScheduledReportsPaged;

public class GetScheduledReportsPagedQueryHandler : IRequestHandler<GetScheduledReportsPagedQuery, PagedResult<ScheduledReportDto>>
{
    private readonly IScheduledReportRepository _scheduledReportRepository;

    public GetScheduledReportsPagedQueryHandler(IScheduledReportRepository scheduledReportRepository)
    {
        _scheduledReportRepository = scheduledReportRepository ?? throw new ArgumentNullException(nameof(scheduledReportRepository));
    }

    public async Task<PagedResult<ScheduledReportDto>> Handle(GetScheduledReportsPagedQuery request, CancellationToken cancellationToken)
    {
        var pageIndex = request.PageIndex < 1 ? 1 : request.PageIndex;
        var pageSize = request.PageSize < 1 ? 10 : (request.PageSize > 100 ? 100 : request.PageSize);

        var allReports = await _scheduledReportRepository.GetAllAsync(cancellationToken);
        var query = allReports.AsEnumerable();

        if (request.ReportType.HasValue)
        {
            query = query.Where(r => r.ReportType == request.ReportType.Value);
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(r => r.IsActive == request.IsActive.Value);
        }

        var totalCount = query.Count();
        var items = query
            .OrderByDescending(r => r.CreatedAtUtc)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(r => new ScheduledReportDto
            {
                Id = r.Id,
                Name = r.Name,
                ReportType = r.ReportType,
                ScheduleFrequency = r.ScheduleFrequency,
                OutputFormat = r.OutputFormat,
                FilterJson = r.FilterJson,
                RecipientEmail = r.RecipientEmail,
                IsActive = r.IsActive,
                LastRunUtc = r.LastRunUtc,
                NextRunUtc = r.NextRunUtc,
                CreatedAtUtc = r.CreatedAtUtc
            })
            .ToList();

        return new PagedResult<ScheduledReportDto>(items, totalCount, pageIndex, pageSize);
    }
}