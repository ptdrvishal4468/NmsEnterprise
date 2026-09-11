using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.ThreatDetection.Dtos;

namespace Nms.Application.ThreatDetection.Commands.AnalyzePortScans;

public class AnalyzePortScansCommandHandler : IRequestHandler<AnalyzePortScansCommand, IReadOnlyList<ThreatIndicatorDto>>
{
    private readonly IPortScanDetector _detector;

    public AnalyzePortScansCommandHandler(IPortScanDetector detector)
    {
        _detector = detector;
    }

    public async Task<IReadOnlyList<ThreatIndicatorDto>> Handle(AnalyzePortScansCommand request, CancellationToken cancellationToken)
    {
        var indicators = await _detector.DetectPortScanIndicatorsAsync(request.TimeWindowMinutes, cancellationToken);

        return indicators.Select(i => new ThreatIndicatorDto
        {
            Id = i.Id,
            TenantId = i.TenantId,
            ThreatType = i.ThreatType,
            Severity = i.Severity,
            Status = i.Status,
            Title = i.Title,
            Description = i.Description,
            SourceIp = i.SourceIp,
            TargetDeviceId = i.TargetDeviceId,
            TargetDeviceName = i.TargetDevice?.Name,
            TargetUser = i.TargetUser,
            AttemptCount = i.AttemptCount,
            FirstDetectedAtUtc = i.FirstDetectedAtUtc,
            LastDetectedAtUtc = i.LastDetectedAtUtc,
            ResolutionNotes = i.ResolutionNotes,
            IndicatorMetadataJson = i.IndicatorMetadataJson
        }).ToList();
    }
}