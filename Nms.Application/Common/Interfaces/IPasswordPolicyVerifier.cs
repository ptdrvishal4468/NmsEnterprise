using Nms.Domain.Entities;

namespace Nms.Application.Common.Interfaces;

/// <summary>
/// Evaluates device authentication strength and password controls.
/// </summary>
public interface IPasswordPolicyVerifier
{
    Task<DeviceComplianceResult> VerifyPasswordPolicyAsync(
        Device device,
        CompliancePolicy policy,
        Guid scanId,
        CancellationToken cancellationToken = default);
}