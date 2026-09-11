using Nms.Domain.Entities;

namespace Nms.Application.Common.Interfaces;

public interface IPortScanDetector
{
    Task<IReadOnlyList<ThreatIndicator>> DetectPortScanIndicatorsAsync(
        int timeWindowMinutes = 60,
        CancellationToken cancellationToken = default);
}