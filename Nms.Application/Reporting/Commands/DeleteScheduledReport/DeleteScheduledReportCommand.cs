using MediatR;

namespace Nms.Application.Reporting.Commands.DeleteScheduledReport;

public record DeleteScheduledReportCommand(Guid Id) : IRequest<bool>;