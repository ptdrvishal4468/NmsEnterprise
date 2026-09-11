using FluentAssertions;
using Nms.Application.Cybersecurity.Services;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Xunit;

namespace Nms.UnitTests.Cybersecurity;

public class EncryptionVerifierTests
{
    private readonly EncryptionVerifier _verifier = new();

    [Fact]
    public async Task VerifyEncryptionAsync_WhenAuthPrivConfigured_ReturnsCompliant()
    {
        var device = new Device(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "EdgeRouter",
            "10.10.10.1",
            DeviceType.Router);

        device.ConfigureSnmpV3Credentials("admin", "AuthKeyAES", "PrivKeyAES");

        var policy = new CompliancePolicy(
            Guid.NewGuid(),
            device.TenantId,
            "SNMPv3 Transport Encryption",
            "Desc",
            ComplianceCategory.Encryption,
            ComplianceCheckType.SnmpV3Security,
            ComplianceSeverity.High);

        var result = await _verifier.VerifyEncryptionAsync(device, policy, Guid.NewGuid());

        result.Status.Should().Be(ComplianceStatus.Compliant);
    }

    [Fact]
    public async Task VerifyEncryptionAsync_WhenOnlyAuthKeyConfigured_ReturnsWarning()
    {
        var device = new Device(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "EdgeRouter",
            "10.10.10.1",
            DeviceType.Router);

        device.ConfigureSnmpV3Credentials("admin", "AuthKeySHA", null!);

        var policy = new CompliancePolicy(
            Guid.NewGuid(),
            device.TenantId,
            "SNMPv3 Transport Encryption",
            "Desc",
            ComplianceCategory.Encryption,
            ComplianceCheckType.SnmpV3Security,
            ComplianceSeverity.High);

        var result = await _verifier.VerifyEncryptionAsync(device, policy, Guid.NewGuid());

        result.Status.Should().Be(ComplianceStatus.Warning);
    }
}