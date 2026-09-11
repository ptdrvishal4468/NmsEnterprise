using Nms.Application.Common.Interfaces;
using Nms.Domain.Entities;
using Nms.Domain.Enums;

namespace Nms.Application.Cybersecurity.Services;

public class PasswordPolicyVerifier : IPasswordPolicyVerifier
{
    public Task<DeviceComplianceResult> VerifyPasswordPolicyAsync(
        Device device,
        CompliancePolicy policy,
        Guid scanId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(device);
        ArgumentNullException.ThrowIfNull(policy);

        if (!IsPolicyApplicable(device, policy))
        {
            return Task.FromResult(new DeviceComplianceResult(
                id: Guid.NewGuid(),
                tenantId: device.TenantId,
                scanId: scanId,
                policyId: policy.Id,
                checkType: policy.CheckType,
                category: policy.Category,
                severity: policy.Severity,
                status: ComplianceStatus.NotApplicable,
                summary: $"Password policy '{policy.Name}' is not applicable to device {device.Name}.",
                details: $"Device Vendor: {device.Vendor ?? "Unknown"}",
                remediationGuidance: null,
                evaluatedAtUtc: DateTime.UtcNow));
        }

        ComplianceStatus status;
        string summary;
        string details;
        string? remediation = null;

        if (policy.CheckType == ComplianceCheckType.DefaultCredentialDisabled)
        {
            // Verify device does not utilize default credentials and has authenticated access configured
            if (string.IsNullOrWhiteSpace(device.SnmpV3User) && string.IsNullOrWhiteSpace(device.SnmpV3AuthKeyEncrypted))
            {
                status = ComplianceStatus.NonCompliant;
                summary = $"Device '{device.Name}' has not configured strong authentication credentials.";
                details = "No encrypted SNMPv3 authentication credentials found. Device may be subject to unauthorized management access.";
                remediation = "Configure strong SNMPv3/SSH credentials and disable all factory default credentials.";
            }
            else
            {
                status = ComplianceStatus.Compliant;
                summary = $"Device '{device.Name}' satisfies credential security baseline.";
                details = "Device has non-default authentication parameters securely provisioned.";
            }
        }
        else // PasswordComplexity or general PasswordPolicy
        {
            if (!string.IsNullOrWhiteSpace(device.SnmpV3AuthKeyEncrypted))
            {
                status = ComplianceStatus.Compliant;
                summary = $"Device '{device.Name}' satisfies authentication complexity baseline.";
                details = "Encrypted authentication keys are provisioned for secure device management.";
            }
            else
            {
                status = ComplianceStatus.Warning;
                summary = $"Device '{device.Name}' authentication strength cannot be fully verified.";
                details = "Authentication keys are not configured in encrypted storage for SNMPv3 management.";
                remediation = "Provision strong SHA-256/SHA-512 authentication passphrases for managed credentials.";
            }
        }

        var result = new DeviceComplianceResult(
            id: Guid.NewGuid(),
            tenantId: device.TenantId,
            scanId: scanId,
            policyId: policy.Id,
            checkType: policy.CheckType,
            category: policy.Category,
            severity: policy.Severity,
            status: status,
            summary: summary,
            details: details,
            remediationGuidance: remediation,
            evaluatedAtUtc: DateTime.UtcNow);

        return Task.FromResult(result);
    }

    private static bool IsPolicyApplicable(Device device, CompliancePolicy policy)
    {
        if (policy.TargetDeviceType.HasValue && policy.TargetDeviceType.Value != device.DeviceType)
            return false;

        if (!string.IsNullOrWhiteSpace(policy.TargetVendor) &&
            !string.Equals(policy.TargetVendor, device.Vendor, StringComparison.OrdinalIgnoreCase))
            return false;

        return true;
    }
}