using FluentAssertions;
using Nms.Application.Cybersecurity.Services;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Xunit;

namespace Nms.UnitTests.Cybersecurity;

public class SecureConfigurationValidatorTests
{
    private readonly SecureConfigurationValidator _validator = new();

    [Fact]
    public async Task ValidateConfigurationAsync_WhenPolicyVendorMismatch_ReturnsNotApplicable()
    {
        var device = new Device(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Switch-1",
            "192.168.1.10",
            DeviceType.Switch,
            vendor: "Juniper");

        var policy = new CompliancePolicy(
            Guid.NewGuid(),
            device.TenantId,
            "Cisco Hardening",
            "Description",
            ComplianceCategory.Configuration,
            ComplianceCheckType.SecureConfiguration,
            ComplianceSeverity.High,
            targetVendor: "Cisco");

        var result = await _validator.ValidateConfigurationAsync(device, policy, Guid.NewGuid());

        result.Status.Should().Be(ComplianceStatus.NotApplicable);
    }

    [Fact]
    public async Task ValidateConfigurationAsync_WhenSnmpV3Configured_ReturnsCompliant()
    {
        var device = new Device(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Router-1",
            "192.168.1.1",
            DeviceType.Router,
            vendor: "Cisco");

        device.ConfigureSnmpV3Credentials("adminUser", "EncryptedAuthKey", "EncryptedPrivKey");
        device.UpdateStatus(DeviceStatus.Online);

        var policy = new CompliancePolicy(
            Guid.NewGuid(),
            device.TenantId,
            "Secure Config Baseline",
            "Description",
            ComplianceCategory.Configuration,
            ComplianceCheckType.SecureConfiguration,
            ComplianceSeverity.Medium);

        var result = await _validator.ValidateConfigurationAsync(device, policy, Guid.NewGuid());

        result.Status.Should().Be(ComplianceStatus.Compliant);
    }
}