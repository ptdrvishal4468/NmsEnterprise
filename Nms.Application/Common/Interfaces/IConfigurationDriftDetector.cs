using Nms.Domain.Entities;

namespace Nms.Application.Common.Interfaces;

public interface IConfigurationDriftDetector
{
    Task<ConfigurationDriftRecord> AnalyzeDeviceDriftAsync(Guid deviceId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ConfigurationDriftRecord>> AnalyzeAllDevicesDriftAsync(CancellationToken cancellationToken = default);
}