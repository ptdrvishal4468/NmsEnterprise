using FluentAssertions;
using Moq;
using Nms.Application.Cybersecurity.Queries.GetCompliancePoliciesPaged;
using Nms.Application.Cybersecurity.Queries.GetCompliancePolicyById;
using Nms.Application.Cybersecurity.Queries.GetCybersecurityPostureSummary;
using Nms.Application.Cybersecurity.Queries.GetDeviceComplianceScanById;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;
using Xunit;

namespace Nms.UnitTests.Cybersecurity;

public class CybersecurityQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ICompliancePolicyRepository> _policyRepoMock = new();
    private readonly Mock<IDeviceComplianceScanRepository> _scanRepoMock = new();
    private readonly Mock<IDeviceRepository> _deviceRepoMock = new();
    private readonly Guid _tenantId = Guid.NewGuid();

    public CybersecurityQueryHandlerTests()
    {
        _unitOfWorkMock.Setup(u => u.CompliancePolicies).Returns(_policyRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.DeviceComplianceScans).Returns(_scanRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Devices).Returns(_deviceRepoMock.Object);
    }

    [Fact]
    public async Task GetCompliancePolicyById_WhenExists_ReturnsDto()
    {
        var id = Guid.NewGuid();
        var policy = new CompliancePolicy(id, _tenantId, "SSH Policy", "Desc", ComplianceCategory.ProtocolSecurity, ComplianceCheckType.InsecureProtocolDisabled, ComplianceSeverity.High);

        _policyRepoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(policy);

        var handler = new GetCompliancePolicyByIdQueryHandler(_unitOfWorkMock.Object);
        var result = await handler.Handle(new GetCompliancePolicyByIdQuery(id), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
        result.Name.Should().Be("SSH Policy");
    }

    [Fact]
    public async Task GetCompliancePoliciesPaged_ReturnsPagedResult()
    {
        var list = new List<CompliancePolicy>
        {
            new(Guid.NewGuid(), _tenantId, "Policy 1", "Desc", ComplianceCategory.Configuration, ComplianceCheckType.SecureConfiguration, ComplianceSeverity.Low),
            new(Guid.NewGuid(), _tenantId, "Policy 2", "Desc", ComplianceCategory.PasswordPolicy, ComplianceCheckType.PasswordComplexity, ComplianceSeverity.Medium)
        };

        _policyRepoMock.Setup(r => r.GetPagedAsync(1, 10, null, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((list, 2));

        var handler = new GetCompliancePoliciesPagedQueryHandler(_unitOfWorkMock.Object);
        var result = await handler.Handle(new GetCompliancePoliciesPagedQuery(1, 10), CancellationToken.None);

        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task GetCybersecurityPostureSummary_ReturnsCalculatedPercentage()
    {
        _scanRepoMock.Setup(r => r.GetPostureSummaryAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((TotalScannedDevices: 10, CompliantDevices: 8, NonCompliantDevices: 1, WarningDevices: 1, UnableToEvaluateDevices: 0));

        var handler = new GetCybersecurityPostureSummaryQueryHandler(_unitOfWorkMock.Object);
        var result = await handler.Handle(new GetCybersecurityPostureSummaryQuery(), CancellationToken.None);

        result.Should().NotBeNull();
        result.TotalScannedDevices.Should().Be(10);
        result.CompliantDevices.Should().Be(8);
        result.CompliancePercentage.Should().Be(80.0);
    }
}