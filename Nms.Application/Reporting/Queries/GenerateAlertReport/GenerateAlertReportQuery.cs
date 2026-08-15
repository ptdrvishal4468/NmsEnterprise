using MediatR;
using Nms.Application.Reporting.Dtos;
using Nms.Domain.Enums;

namespace Nms.Application.Reporting.Queries.GenerateAlertReport;

public record GenerateAlertReportQuery(
    AlertSeverity? Severity = null,
    AlertState? State = null,
    MetricType? MetricType = null,
    DateTime? FromUtc = null,
    DateTime? ToUtc = null,
    ReportFormat Format = ReportFormat.Json) : IRequest<ReportExportResultDto>;