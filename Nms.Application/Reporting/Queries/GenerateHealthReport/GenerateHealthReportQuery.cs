using MediatR;
using Nms.Application.Reporting.Dtos;
using Nms.Domain.Enums;

namespace Nms.Application.Reporting.Queries.GenerateHealthReport;

public record GenerateHealthReportQuery(
    DeviceStatus? Status = null,
    double? MinHealthScore = null,
    double? MaxHealthScore = null,
    DateTime? FromUtc = null,
    DateTime? ToUtc = null,
    ReportFormat Format = ReportFormat.Json) : IRequest<ReportExportResultDto>;