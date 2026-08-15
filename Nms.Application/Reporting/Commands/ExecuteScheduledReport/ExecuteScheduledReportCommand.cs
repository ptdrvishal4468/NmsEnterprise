using MediatR;
using Nms.Application.Reporting.Dtos;

namespace Nms.Application.Reporting.Commands.ExecuteScheduledReport;

public record ExecuteScheduledReportCommand(Guid ScheduledReportId) : IRequest<ScheduledReportExecutionLogDto>;