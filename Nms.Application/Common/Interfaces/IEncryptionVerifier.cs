using Nms.Domain.Entities;

namespace Nms.Application.Common.Interfaces;

/// <summary>
/// Verifies transport encryption and secure management protocols (e.g. SSH vs Telnet, SNMPv3 vs v1/v2c).
/// </summary>
public interface IEncryptionVerifier
{
    Task<DeviceComplianceResult> VerifyEncryptionAsync(
        Device device,
        CompliancePolicy policy,
        Guid scanId,
        CancellationToken cancellationToken = default);
}