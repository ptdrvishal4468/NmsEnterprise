using Nms.Domain.Entities;

namespace Nms.Application.Common.Interfaces;

public interface IFailedLoginDetector
{
    Task<IReadOnlyList<ThreatIndicator>> DetectFailedLoginsAsync(
        int? thresholdOverride = null,
        int? timeWindowMinutesOverride = null,
        CancellationToken cancellationToken = default);
}