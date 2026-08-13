using Moq;
using Nms.Application.Common.Interfaces;
using Nms.Application.Firmware.Queries.GetFirmwareComplianceSummary;
using Nms.Application.Firmware.Services;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;
using Xunit;

namespace Nms.UnitTests.Firmware;

public class GetFirmwareComplianceSummaryQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ITenantContext> _tenantContextMock = new();
    private readonly Mock<IDeviceRepository> _deviceRepoMock = new();
    private readonly Mock<IFirmwareBaselineRepository> _baselineRepoMock = new();
    private readonly IFirmwareVersionComparator _versionComparator = new FirmwareVersionComparator();
    private readonly Guid _tenantId = Guid.NewGuid();

    public GetFirmwareComplianceSummaryQueryHandlerTests()
    {
        _tenantContextMock.Setup(t => t.TenantId).Returns(_tenantId);
        _unitOfWorkMock.Setup(u => u.Devices).Returns(_deviceRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.FirmwareBaselines).Returns(_baselineRepoMock.Object);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Handle_ShouldCalculateComplianceSummaryCorrectly_WithMixedDevices()
    {
        var devices = new List<Device>
        {
            new Device(Guid.NewGuid(), _tenantId, "Core-1", "10.0.0.1", DeviceType.Switch, vendor: "Cisco", model: "Catalyst 9300", firmwareVersion: "17.3.4"),
            new Device(Guid.NewGuid(), _tenantId, "Core-2", "10.0.0.2", DeviceType.Switch, vendor: "Cisco", model: "Catalyst 9300", firmwareVersion: "17.1.0"),
            new Device(Guid.NewGuid(), _tenantId, "Edge-1", "10.0.0.3", DeviceType.Router, vendor: "Juniper", model: "MX240", firmwareVersion: "21.4R1")
        };

        var baselines = new List<FirmwareBaseline>
        {
            new FirmwareBaseline(Guid.NewGuid(), _tenantId, "Cisco", "Catalyst 9300", "17.3.4")
        };

        _deviceRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(devices);

        _baselineRepoMock.Setup(r => r.GetActiveBaselinesAsync(_tenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(baselines);

        var handler = new GetFirmwareComplianceSummaryQueryHandler(_unitOfWorkMock.Object, _tenantContextMock.Object, _versionComparator);
        var query = new GetFirmwareComplianceSummaryQuery();

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(3, result.TotalDevices);
        Assert.Equal(1, result.CompliantDevices);
        Assert.Equal(1, result.NonCompliantDevices);
        Assert.Equal(1, result.UnknownDevices);
        Assert.Equal(33.33, result.CompliancePercentage);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Handle_ShouldReturnZeroSummary_WhenNoDevicesExist()
    {
        _deviceRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Device>());

        _baselineRepoMock.Setup(r => r.GetActiveBaselinesAsync(_tenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FirmwareBaseline>());

        var handler = new GetFirmwareComplianceSummaryQueryHandler(_unitOfWorkMock.Object, _tenantContextMock.Object, _versionComparator);
        var query = new GetFirmwareComplianceSummaryQuery();

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(0, result.TotalDevices);
        Assert.Equal(0, result.CompliantDevices);
        Assert.Equal(0, result.NonCompliantDevices);
        Assert.Equal(0, result.UnknownDevices);
        Assert.Equal(0.0, result.CompliancePercentage);
    }
}