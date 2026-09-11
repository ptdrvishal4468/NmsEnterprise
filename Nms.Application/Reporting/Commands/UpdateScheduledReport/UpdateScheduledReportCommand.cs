using MediatR;
using Nms.Domain.Enums;

namespace Nms.Application.Reporting.Commands.UpdateScheduledReport;

public record UpdateScheduledReportCommand(
    Guid Id,
    string Name,
    ReportType ReportType,
    ReportScheduleFrequency ScheduleFrequency,
    ReportFormat OutputFormat,
    string RecipientEmail,
    bool IsActive,
    string? FilterJson = null) : IRequest<bool>;