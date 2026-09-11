using MediatR;
using Nms.Domain.Enums;

namespace Nms.Application.Reporting.Commands.CreateScheduledReport;

public record CreateScheduledReportCommand(
    string Name,
    ReportType ReportType,
    ReportScheduleFrequency ScheduleFrequency,
    ReportFormat OutputFormat,
    string RecipientEmail,
    string? FilterJson = null) : IRequest<Guid>;