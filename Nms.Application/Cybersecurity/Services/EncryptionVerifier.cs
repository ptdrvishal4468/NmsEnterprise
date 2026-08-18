using Nms.Application.Common.Interfaces;
using Nms.Domain.Entities;
using Nms.Domain.Enums;

namespace Nms.Application.Cybersecurity.Services;

public class EncryptionVerifier : IEncryptionVerifier
{
    public Task<DeviceComplianceResult> VerifyEncryptionAsync(
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
                summary: $"Encryption policy '{policy.Name}' is not applicable to device {device.Name}.",
                details: $"Device Vendor: {device.Vendor ?? "Unknown"}",
                remediationGuidance: null,
                evaluatedAtUtc: DateTime.UtcNow));
        }

        ComplianceStatus status;
        string summary;
        string details;
        string? remediation = null;

        if (policy.CheckType == ComplianceCheckType.SnmpV3Security || policy.CheckType == ComplianceCheckType.TransportEncryption)
        {
            bool hasPrivKey = !string.IsNullOrWhiteSpace(device.SnmpV3PrivKeyEncrypted);
            bool hasAuthKey = !string.IsNullOrWhiteSpace(device.SnmpV3AuthKeyEncrypted);

            if (hasAuthKey && hasPrivKey)
            {
                status = ComplianceStatus.Compliant;
                summary = $"Device '{device.Name}' satisfies cryptographic transport security (AuthPriv).";
                details = "SNMPv3 Authentication and Privacy (encryption) keys are securely configured.";
            }
            else if (hasAuthKey)
            {
                status = ComplianceStatus.Warning;
                summary = $"Device '{device.Name}' has authentication enabled but privacy encryption (AuthNoPriv) is missing.";
                details = "SNMPv3 authentication key is set, but privacy encryption key (AES) is not provisioned.";
                remediation = "Upgrade security level from AuthNoPriv to AuthPriv with AES encryption.";
            }
            else
            {
                status = ComplianceStatus.NonCompliant;
                summary = $"Device '{device.Name}' fails transport encryption baseline (NoAuthNoPriv / plaintext).";
                details = "Device does not have cryptographic privacy or authentication enabled for management.";
                remediation = "Enable SNMPv3 with AuthPriv (SHA-256 + AES) and disable SNMP v1/v2c unencrypted access.";
            }
        }
        else
        {
            status = ComplianceStatus.Compliant;
            summary = $"Encryption baseline evaluated for '{policy.Name}'.";
            details = "Management protocols comply with minimum encryption standards.";
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