using FluentAssertions;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Xunit;

namespace Nms.UnitTests.Cybersecurity;

public class CompliancePolicyDomainTests
{
    [Fact]
    public void Constructor_WithValidArguments_InitializesCorrectly()
    {
        var id = Guid.NewGuid();
        var tenantId = Guid.NewGuid();

        var policy = new CompliancePolicy(
            id,
            tenantId,
            "Baseline Password Policy",
            "Enforce strong authentication",
            ComplianceCategory.PasswordPolicy,
            ComplianceCheckType.PasswordComplexity,
            ComplianceSeverity.High,
            isActive: true,
            targetVendor: "Cisco",
            targetDeviceType: DeviceType.Router);

        policy.Id.Should().Be(id);
        policy.TenantId.Should().Be(tenantId);
        policy.Name.Should().Be("Baseline Password Policy");
        policy.Description.Should().Be("Enforce strong authentication");
        policy.Category.Should().Be(ComplianceCategory.PasswordPolicy);
        policy.CheckType.Should().Be(ComplianceCheckType.PasswordComplexity);
        policy.Severity.Should().Be(ComplianceSeverity.High);
        policy.IsActive.Should().BeTrue();
        policy.TargetVendor.Should().Be("Cisco");
        policy.TargetDeviceType.Should().Be(DeviceType.Router);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_WithInvalidName_ThrowsArgumentException(string? invalidName)
    {
        var act = () => new CompliancePolicy(
            Guid.NewGuid(),
            Guid.NewGuid(),
            invalidName!,
            "Desc",
            ComplianceCategory.Configuration,
            ComplianceCheckType.SecureConfiguration,
            ComplianceSeverity.Medium);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Scan_RecalculateTotals_CalculatesStatusAccurately()
    {
        var scanId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var deviceId = Guid.NewGuid();
        var policyId = Guid.NewGuid();

        var scan = new DeviceComplianceScan(scanId, tenantId, deviceId, DateTime.UtcNow);

        // Initially unable to evaluate
        scan.RecalculateTotals();
        scan.OverallStatus.Should().Be(ComplianceStatus.UnableToEvaluate);

        // Add 1 compliant check
        var result1 = new DeviceComplianceResult(
            Guid.NewGuid(),
            tenantId,
            scanId,
            policyId,
            ComplianceCheckType.SecureConfiguration,
            ComplianceCategory.Configuration,
            ComplianceSeverity.Medium,
            ComplianceStatus.Compliant,
            "Compliant check");

        scan.AddResult(result1);
        scan.OverallStatus.Should().Be(ComplianceStatus.Compliant);
        scan.PassedChecks.Should().Be(1);

        // Add 1 warning check
        var result2 = new DeviceComplianceResult(
            Guid.NewGuid(),
            tenantId,
            scanId,
            policyId,
            ComplianceCheckType.SnmpV3Security,
            ComplianceCategory.Encryption,
            ComplianceSeverity.High,
            ComplianceStatus.Warning,
            "Warning check");

        scan.AddResult(result2);
        scan.OverallStatus.Should().Be(ComplianceStatus.Warning);
        scan.WarningChecks.Should().Be(1);

        // Add 1 non-compliant check
        var result3 = new DeviceComplianceResult(
            Guid.NewGuid(),
            tenantId,
            scanId,
            policyId,
            ComplianceCheckType.InsecureProtocolDisabled,
            ComplianceCategory.ProtocolSecurity,
            ComplianceSeverity.Critical,
            ComplianceStatus.NonCompliant,
            "Failed check");

        scan.AddResult(result3);
        scan.OverallStatus.Should().Be(ComplianceStatus.NonCompliant);
        scan.FailedChecks.Should().Be(1);
        scan.TotalChecks.Should().Be(3);
    }
}