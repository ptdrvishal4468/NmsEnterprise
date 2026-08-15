using MediatR;
using Nms.Domain.Interfaces;

namespace Nms.Application.Reporting.Commands.DeleteScheduledReport;

public class DeleteScheduledReportCommandHandler : IRequestHandler<DeleteScheduledReportCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteScheduledReportCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<bool> Handle(DeleteScheduledReportCommand request, CancellationToken cancellationToken)
    {
        var scheduledReport = await _unitOfWork.ScheduledReports.GetByIdAsync(request.Id, cancellationToken);
        if (scheduledReport == null)
        {
            throw new KeyNotFoundException($"Scheduled report with ID '{request.Id}' was not found.");
        }

        _unitOfWork.ScheduledReports.Remove(scheduledReport);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}