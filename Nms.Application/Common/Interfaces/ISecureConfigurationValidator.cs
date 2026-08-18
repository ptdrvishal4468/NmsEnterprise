using Nms.Domain.Entities;

namespace Nms.Application.Common.Interfaces;

/// <summary>
/// Verifies device configuration against established security baselines without credential exposure.
/// </summary>
public interface ISecureConfigurationValidator
{
    Task<DeviceComplianceResult> ValidateConfigurationAsync(
        Device device,
        CompliancePolicy policy,
        Guid scanId,
        CancellationToken cancellationToken = default);
}