using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.ThreatDetection.Dtos;

namespace Nms.Application.ThreatDetection.Commands.AnalyzeFailedLogins;

public class AnalyzeFailedLoginsCommandHandler : IRequestHandler<AnalyzeFailedLoginsCommand, IReadOnlyList<ThreatIndicatorDto>>
{
    private readonly IFailedLoginDetector _detector;

    public AnalyzeFailedLoginsCommandHandler(IFailedLoginDetector detector)
    {
        _detector = detector;
    }

    public async Task<IReadOnlyList<ThreatIndicatorDto>> Handle(AnalyzeFailedLoginsCommand request, CancellationToken cancellationToken)
    {
        var indicators = await _detector.DetectFailedLoginsAsync(request.ThresholdOverride, request.TimeWindowMinutesOverride, cancellationToken);

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