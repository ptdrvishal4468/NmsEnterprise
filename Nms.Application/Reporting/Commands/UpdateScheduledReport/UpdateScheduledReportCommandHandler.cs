using MediatR;
using Nms.Domain.Interfaces;

namespace Nms.Application.Reporting.Commands.UpdateScheduledReport;

public class UpdateScheduledReportCommandHandler : IRequestHandler<UpdateScheduledReportCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateScheduledReportCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<bool> Handle(UpdateScheduledReportCommand request, CancellationToken cancellationToken)
    {
        var scheduledReport = await _unitOfWork.ScheduledReports.GetByIdAsync(request.Id, cancellationToken);
        if (scheduledReport == null)
        {
            throw new KeyNotFoundException($"Scheduled report with ID '{request.Id}' was not found.");
        }

        scheduledReport.Update(
            request.Name,
            request.ReportType,
            request.ScheduleFrequency,
            request.OutputFormat,
            request.RecipientEmail,
            request.FilterJson);

        scheduledReport.ToggleActive(request.IsActive);

        _unitOfWork.ScheduledReports.Update(scheduledReport);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}