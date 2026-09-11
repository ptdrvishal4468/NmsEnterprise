using Nms.Domain.Entities;

namespace Nms.Application.Common.Interfaces;

public interface IUnauthorizedAccessDetector
{
    Task<IReadOnlyList<ThreatIndicator>> DetectUnauthorizedAccessPatternsAsync(
        int timeWindowMinutes = 60,
        CancellationToken cancellationToken = default);
}