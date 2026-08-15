using MediatR;
using Nms.Application.Reporting.Dtos;
using Nms.Application.Reporting.Queries.GenerateAlertReport;
using Nms.Application.Reporting.Queries.GenerateDeviceReport;
using Nms.Application.Reporting.Queries.GenerateHealthReport;
using Nms.Application.Reporting.Queries.GenerateInventoryReport;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;

namespace Nms.Application.Reporting.Commands.ExecuteScheduledReport;

public class ExecuteScheduledReportCommandHandler : IRequestHandler<ExecuteScheduledReportCommand, ScheduledReportExecutionLogDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public ExecuteScheduledReportCommandHandler(
        IUnitOfWork unitOfWork,
        IMediator mediator)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<ScheduledReportExecutionLogDto> Handle(ExecuteScheduledReportCommand request, CancellationToken cancellationToken)
    {
        var scheduledReport = await _unitOfWork.ScheduledReports.GetByIdAsync(request.ScheduledReportId, cancellationToken);
        if (scheduledReport == null)
        {
            throw new KeyNotFoundException($"Scheduled report with ID '{request.ScheduledReportId}' was not found.");
        }

        var executionTimeUtc = DateTime.UtcNow;
        var status = ReportExecutionStatus.Success;
        var recordCount = 0;
        string? errorMessage = null;

        try
        {
            ReportExportResultDto result = scheduledReport.ReportType switch
            {
                ReportType.Device => await _mediator.Send(new GenerateDeviceReportQuery(Format: scheduledReport.OutputFormat), cancellationToken),
                ReportType.Health => await _mediator.Send(new GenerateHealthReportQuery(Format: scheduledReport.OutputFormat), cancellationToken),
                ReportType.Inventory => await _mediator.Send(new GenerateInventoryReportQuery(Format: scheduledReport.OutputFormat), cancellationToken),
                ReportType.Alert => await _mediator.Send(new GenerateAlertReportQuery(Format: scheduledReport.OutputFormat), cancellationToken),
                _ => throw new InvalidOperationException($"Unsupported report type: {scheduledReport.ReportType}")
            };

            recordCount = result.TotalRecords;
            scheduledReport.RecordExecution(executionTimeUtc);
        }
        catch (Exception ex)
        {
            status = ReportExecutionStatus.Failed;
            errorMessage = ex.Message;
        }

        var log = new ScheduledReportExecutionLog(
            Guid.NewGuid(),
            scheduledReport.TenantId,
            scheduledReport.Id,
            executionTimeUtc,
            status,
            recordCount,
            errorMessage);

        await _unitOfWork.ScheduledReportExecutionLogs.AddAsync(log, cancellationToken);
        _unitOfWork.ScheduledReports.Update(scheduledReport);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ScheduledReportExecutionLogDto
        {
            Id = log.Id,
            ScheduledReportId = log.ScheduledReportId,
            ExecutedAtUtc = log.ExecutedAtUtc,
            Status = log.Status,
            RecordCount = log.RecordCount,
            ErrorMessage = log.ErrorMessage
        };
    }
}