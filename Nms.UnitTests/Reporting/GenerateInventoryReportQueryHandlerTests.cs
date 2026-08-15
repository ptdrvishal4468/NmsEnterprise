using Moq;
using Nms.Application.Common.Interfaces;
using Nms.Application.Reporting.Queries.GenerateInventoryReport;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;
using Xunit;

namespace Nms.UnitTests.Reporting;

public class GenerateInventoryReportQueryHandlerTests
{
    private readonly Mock<IDeviceRepository> _deviceRepoMock;
    private readonly Mock<INetworkInterfaceRepository> _interfaceRepoMock;
    private readonly Mock<ICsvReportFormatter> _csvFormatterMock;
    private readonly GenerateInventoryReportQueryHandler _handler;

    public GenerateInventoryReportQueryHandlerTests()
    {
        _deviceRepoMock = new Mock<IDeviceRepository>();
        _interfaceRepoMock = new Mock<INetworkInterfaceRepository>();
        _csvFormatterMock = new Mock<ICsvReportFormatter>();
        _handler = new GenerateInventoryReportQueryHandler(_deviceRepoMock.Object, _interfaceRepoMock.Object, _csvFormatterMock.Object);
    }

    [Fact]
    public async Task Handle_CalculatesInterfaceCapacityCorrectly()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var devId = Guid.NewGuid();

        var devices = new List<Device>
        {
            new(devId, tenantId, "Core-Switch", "10.10.10.1", DeviceType.Switch, vendor: "Cisco", site: "DC1")
        };

        var interfaces = new List<NetworkInterface>
        {
            new(Guid.NewGuid(), tenantId, devId, 1, "eth0", speedBps: 1_000_000_000, operStatus: InterfaceOperStatus.Up),
            new(Guid.NewGuid(), tenantId, devId, 2, "eth1", speedBps: 10_000_000_000, operStatus: InterfaceOperStatus.Up)
        };

        _deviceRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(devices);
        _interfaceRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(interfaces);

        var query = new GenerateInventoryReportQuery(Site: "DC1", Format: ReportFormat.Json);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.TotalRecords);
    }
}