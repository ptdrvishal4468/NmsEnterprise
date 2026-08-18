using MediatR;
using Nms.Application.Cybersecurity.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Cybersecurity.Queries.GetCybersecurityPostureSummary;

public class GetCybersecurityPostureSummaryQueryHandler : IRequestHandler<GetCybersecurityPostureSummaryQuery, CybersecurityPostureSummaryDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCybersecurityPostureSummaryQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CybersecurityPostureSummaryDto> Handle(GetCybersecurityPostureSummaryQuery request, CancellationToken cancellationToken)
    {
        var summary = await _unitOfWork.DeviceComplianceScans.GetPostureSummaryAsync(cancellationToken);

        double percentage = summary.TotalScannedDevices > 0
            ? Math.Round((double)summary.CompliantDevices / summary.TotalScannedDevices * 100.0, 2)
            : 0.0;

        return new CybersecurityPostureSummaryDto
        {
            TotalScannedDevices = summary.TotalScannedDevices,
            CompliantDevices = summary.CompliantDevices,
            NonCompliantDevices = summary.NonCompliantDevices,
            WarningDevices = summary.WarningDevices,
            UnableToEvaluateDevices = summary.UnableToEvaluateDevices,
            CompliancePercentage = percentage,
            GeneratedAtUtc = DateTime.UtcNow
        };
    }
}