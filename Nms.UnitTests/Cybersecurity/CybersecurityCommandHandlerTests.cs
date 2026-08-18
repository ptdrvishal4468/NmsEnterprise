using FluentAssertions;
using Moq;
using Nms.Application.Common.Interfaces;
using Nms.Application.Cybersecurity.Commands.CreateCompliancePolicy;
using Nms.Application.Cybersecurity.Commands.DeleteCompliancePolicy;
using Nms.Application.Cybersecurity.Commands.EvaluateDeviceCompliance;
using Nms.Application.Cybersecurity.Commands.UpdateCompliancePolicy;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;
using Xunit;

namespace Nms.UnitTests.Cybersecurity;

public class CybersecurityCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ICompliancePolicyRepository> _policyRepoMock = new();
    private readonly Mock<IDeviceComplianceScanRepository> _scanRepoMock = new();
    private readonly Mock<IDeviceRepository> _deviceRepoMock = new();
    private readonly Mock<ITenantContext> _tenantContextMock = new();
    private readonly Mock<ICybersecurityComplianceEngine> _complianceEngineMock = new();
    private readonly Guid _tenantId = Guid.NewGuid();

    public CybersecurityCommandHandlerTests()
    {
        _tenantContextMock.Setup(t => t.TenantId).Returns(_tenantId);
        _tenantContextMock.Setup(t => t.IsResolved).Returns(true);

        _unitOfWorkMock.Setup(u => u.CompliancePolicies).Returns(_policyRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.DeviceComplianceScans).Returns(_scanRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Devices).Returns(_deviceRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
    }

    [Fact]
    public async Task CreateCompliancePolicy_WithValidData_PersistsAndReturnsDto()
    {
        var handler = new CreateCompliancePolicyCommandHandler(_unitOfWorkMock.Object, _tenantContextMock.Object);
        var command = new CreateCompliancePolicyCommand(
            "Baseline SSH Policy",
            "Enforces SSHv2",
            ComplianceCategory.ProtocolSecurity,
            ComplianceCheckType.InsecureProtocolDisabled,
            ComplianceSeverity.High);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Name.Should().Be("Baseline SSH Policy");
        result.TenantId.Should().Be(_tenantId);
        result.Severity.Should().Be(ComplianceSeverity.High);

        _policyRepoMock.Verify(r => r.AddAsync(It.IsAny<CompliancePolicy>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateCompliancePolicy_WhenPolicyExists_UpdatesAndReturnsDto()
    {
        var policyId = Guid.NewGuid();
        var existingPolicy = new CompliancePolicy(
            policyId,
            _tenantId,
            "Initial Policy",
            "Desc",
            ComplianceCategory.Configuration,
            ComplianceCheckType.SecureConfiguration,
            ComplianceSeverity.Low);

        _policyRepoMock.Setup(r => r.GetByIdAsync(policyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingPolicy);

        var handler = new UpdateCompliancePolicyCommandHandler(_unitOfWorkMock.Object);
        var command = new UpdateCompliancePolicyCommand(
            policyId,
            "Updated Policy Name",
            "Updated Desc",
            ComplianceCategory.Configuration,
            ComplianceCheckType.SecureConfiguration,
            ComplianceSeverity.Critical,
            true);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Updated Policy Name");
        result.Severity.Should().Be(ComplianceSeverity.Critical);

        _policyRepoMock.Verify(r => r.Update(It.IsAny<CompliancePolicy>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteCompliancePolicy_WhenExists_RemovesAndReturnsTrue()
    {
        var policyId = Guid.NewGuid();
        var existingPolicy = new CompliancePolicy(
            policyId,
            _tenantId,
            "To Delete",
            "Desc",
            ComplianceCategory.Configuration,
            ComplianceCheckType.SecureConfiguration,
            ComplianceSeverity.Low);

        _policyRepoMock.Setup(r => r.GetByIdAsync(policyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingPolicy);

        var handler = new DeleteCompliancePolicyCommandHandler(_unitOfWorkMock.Object);
        var result = await handler.Handle(new DeleteCompliancePolicyCommand(policyId), CancellationToken.None);

        result.Should().BeTrue();
        _policyRepoMock.Verify(r => r.Remove(existingPolicy), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task EvaluateDeviceCompliance_WhenDeviceFound_ExecutesEvaluationEngine()
    {
        var deviceId = Guid.NewGuid();
        var device = new Device(deviceId, _tenantId, "Core-Router", "192.168.1.1", DeviceType.Router);
        var policies = new List<CompliancePolicy>
        {
            new(Guid.NewGuid(), _tenantId, "Policy 1", "Desc", ComplianceCategory.Encryption, ComplianceCheckType.TransportEncryption, ComplianceSeverity.High)
        };

        var scan = new DeviceComplianceScan(Guid.NewGuid(), _tenantId, deviceId, DateTime.UtcNow);
        scan.AddResult(new DeviceComplianceResult(Guid.NewGuid(), _tenantId, scan.Id, policies[0].Id, ComplianceCheckType.TransportEncryption, ComplianceCategory.Encryption, ComplianceSeverity.High, ComplianceStatus.Compliant, "Passed"));

        _deviceRepoMock.Setup(r => r.GetByIdAsync(deviceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(device);
        _policyRepoMock.Setup(r => r.GetActivePoliciesForDeviceTypeAsync(DeviceType.Router, device.Vendor, It.IsAny<CancellationToken>()))
            .ReturnsAsync(policies);
        _complianceEngineMock.Setup(e => e.EvaluateDeviceAsync(device, policies, It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(scan);

        var handler = new EvaluateDeviceComplianceCommandHandler(_unitOfWorkMock.Object, _complianceEngineMock.Object);
        var result = await handler.Handle(new EvaluateDeviceComplianceCommand(deviceId, "Automated check"), CancellationToken.None);

        result.Should().NotBeNull();
        result!.DeviceId.Should().Be(deviceId);
        result.OverallStatus.Should().Be(ComplianceStatus.Compliant);
        result.PassedChecks.Should().Be(1);

        _scanRepoMock.Verify(r => r.AddAsync(It.IsAny<DeviceComplianceScan>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}