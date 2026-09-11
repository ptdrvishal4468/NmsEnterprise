using FluentAssertions;
using Nms.Application.Cybersecurity.Services;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Xunit;

namespace Nms.UnitTests.Cybersecurity;

public class PasswordPolicyVerifierTests
{
    private readonly PasswordPolicyVerifier _verifier = new();

    [Fact]
    public async Task VerifyPasswordPolicyAsync_WhenNoCredentialsConfigured_ReturnsNonCompliantForDefaultCheck()
    {
        var device = new Device(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "CoreSwitch",
            "10.0.0.1",
            DeviceType.Switch);

        var policy = new CompliancePolicy(
            Guid.NewGuid(),
            device.TenantId,
            "Disable Default Passwords",
            "Desc",
            ComplianceCategory.PasswordPolicy,
            ComplianceCheckType.DefaultCredentialDisabled,
            ComplianceSeverity.Critical);

        var result = await _verifier.VerifyPasswordPolicyAsync(device, policy, Guid.NewGuid());

        result.Status.Should().Be(ComplianceStatus.NonCompliant);
    }

    [Fact]
    public async Task VerifyPasswordPolicyAsync_WhenEncryptedAuthKeyProvided_ReturnsCompliant()
    {
        var device = new Device(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "CoreSwitch",
            "10.0.0.1",
            DeviceType.Switch);

        device.ConfigureSnmpV3Credentials("nms_admin", "EncryptedAuthKey", "EncryptedPrivKey");

        var policy = new CompliancePolicy(
            Guid.NewGuid(),
            device.TenantId,
            "Complexity Check",
            "Desc",
            ComplianceCategory.PasswordPolicy,
            ComplianceCheckType.PasswordComplexity,
            ComplianceSeverity.High);

        var result = await _verifier.VerifyPasswordPolicyAsync(device, policy, Guid.NewGuid());

        result.Status.Should().Be(ComplianceStatus.Compliant);
    }
}