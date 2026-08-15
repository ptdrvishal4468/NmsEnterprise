using MediatR;
using Nms.Application.Reporting.Dtos;
using Nms.Domain.Enums;

namespace Nms.Application.Reporting.Queries.GenerateInventoryReport;

public record GenerateInventoryReportQuery(
    string? Vendor = null,
    string? Site = null,
    ReportFormat Format = ReportFormat.Json) : IRequest<ReportExportResultDto>;