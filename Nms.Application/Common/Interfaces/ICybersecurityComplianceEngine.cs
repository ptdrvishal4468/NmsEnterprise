using Nms.Domain.Entities;

namespace Nms.Application.Common.Interfaces;

/// <summary>
/// Orchestrates security posture evaluations across all active compliance policies for a device.
/// </summary>
public interface ICybersecurityComplianceEngine
{
    Task<DeviceComplianceScan> EvaluateDeviceAsync(
        Device device,
        IReadOnlyList<CompliancePolicy> policies,
        string? evaluationNotes = null,
        CancellationToken cancellationToken = default);
}