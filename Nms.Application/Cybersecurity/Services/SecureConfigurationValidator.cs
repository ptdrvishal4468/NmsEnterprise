using Nms.Application.Common.Interfaces;
using Nms.Domain.Entities;
using Nms.Domain.Enums;

namespace Nms.Application.Cybersecurity.Services;

public class SecureConfigurationValidator : ISecureConfigurationValidator
{
    public Task<DeviceComplianceResult> ValidateConfigurationAsync(
        Device device,
        CompliancePolicy policy,
        Guid scanId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(device);
        ArgumentNullException.ThrowIfNull(policy);

        // Filter applicability based on target vendor and device type
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
                summary: $"Policy '{policy.Name}' is not applicable to device {device.Name} ({device.DeviceType}).",
                details: $"Device Vendor: {device.Vendor ?? "Unknown"}, Device Type: {device.DeviceType}",
                remediationGuidance: null,
                evaluatedAtUtc: DateTime.UtcNow));
        }

        // Configuration evaluation: Check standard non-secure port exposures and configuration posture
        var issues = new List<string>();
        var status = ComplianceStatus.Compliant;
        string? remediation = null;

        if (policy.CheckType == ComplianceCheckType.SecureConfiguration)
        {
            if (device.SnmpPort == 161 && string.IsNullOrEmpty(device.SnmpV3User))
            {
                issues.Add("Device is running legacy SNMP service on default UDP port 161 without configured SNMPv3 user.");
                status = ComplianceStatus.Warning;
                remediation = "Migrate device SNMP service to SNMPv3 or restrict management access to secure subnets.";
            }

            if (device.Status == DeviceStatus.Unknown)
            {
                status = ComplianceStatus.UnableToEvaluate;
                issues.Add("Device reachability state is unknown; full configuration audit could not be verified.");
                remediation = "Ensure the device is reachable and has active inventory/reachability status.";
            }
        }
        else if (policy.CheckType == ComplianceCheckType.InsecureProtocolDisabled)
        {
            if (device.SnmpPort != 161 && string.IsNullOrEmpty(device.SnmpV3User))
            {
                issues.Add("Legacy unencrypted protocol port is active without cryptographic safeguards.");
                status = ComplianceStatus.NonCompliant;
                remediation = "Disable unencrypted management protocols and enable SSH / SNMPv3.";
            }
        }

        string summary = status switch
        {
            ComplianceStatus.Compliant => $"Device '{device.Name}' complies with configuration policy '{policy.Name}'.",
            ComplianceStatus.Warning => $"Device '{device.Name}' has potential security configuration risks under policy '{policy.Name}'.",
            ComplianceStatus.NonCompliant => $"Device '{device.Name}' failed secure configuration verification for policy '{policy.Name}'.",
            _ => $"Device '{device.Name}' evaluation status for policy '{policy.Name}' is {status}."
        };

        string details = issues.Count > 0
            ? string.Join("; ", issues)
            : $"Verified configuration baseline for IP: {device.IpAddress}, Device Type: {device.DeviceType}.";

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