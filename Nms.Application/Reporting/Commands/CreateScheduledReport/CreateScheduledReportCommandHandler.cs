using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Application.Reporting.Commands.CreateScheduledReport;

public class CreateScheduledReportCommandHandler : IRequestHandler<CreateScheduledReportCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;

    public CreateScheduledReportCommandHandler(
        IUnitOfWork unitOfWork,
        ITenantContext tenantContext)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
    }

    public async Task<Guid> Handle(CreateScheduledReportCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var scheduledReport = new ScheduledReport(
            Guid.NewGuid(),
            tenantId,
            request.Name,
            request.ReportType,
            request.ScheduleFrequency,
            request.OutputFormat,
            request.RecipientEmail,
            request.FilterJson);

        await _unitOfWork.ScheduledReports.AddAsync(scheduledReport, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return scheduledReport.Id;
    }
}