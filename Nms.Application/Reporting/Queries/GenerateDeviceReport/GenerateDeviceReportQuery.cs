using MediatR;
using Nms.Application.Reporting.Dtos;
using Nms.Domain.Enums;

namespace Nms.Application.Reporting.Queries.GenerateDeviceReport;

public record GenerateDeviceReportQuery(
    DeviceType? DeviceType = null,
    DeviceStatus? Status = null,
    string? Vendor = null,
    string? Site = null,
    ReportFormat Format = ReportFormat.Json) : IRequest<ReportExportResultDto>;