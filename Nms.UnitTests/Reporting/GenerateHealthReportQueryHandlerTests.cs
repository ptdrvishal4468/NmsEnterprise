using Moq;
using Nms.Application.Common.Interfaces;
using Nms.Application.Reporting.Queries.GenerateHealthReport;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;
using Xunit;

namespace Nms.UnitTests.Reporting;

public class GenerateHealthReportQueryHandlerTests
{
    private readonly Mock<IDeviceHealthHistoryRepository> _healthHistoryRepoMock;
    private readonly Mock<IDeviceRepository> _deviceRepoMock;
    private readonly Mock<ICsvReportFormatter> _csvFormatterMock;
    private readonly GenerateHealthReportQueryHandler _handler;

    public GenerateHealthReportQueryHandlerTests()
    {
        _healthHistoryRepoMock = new Mock<IDeviceHealthHistoryRepository>();
        _deviceRepoMock = new Mock<IDeviceRepository>();
        _csvFormatterMock = new Mock<ICsvReportFormatter>();
        _handler = new GenerateHealthReportQueryHandler(_healthHistoryRepoMock.Object, _deviceRepoMock.Object, _csvFormatterMock.Object);
    }

    [Fact]
    public async Task Handle_FiltersByHealthScore_ReturnsFilteredResults()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var devId1 = Guid.NewGuid();
        var devId2 = Guid.NewGuid();

        var devices = new List<Device>
        {
            new(devId1, tenantId, "Router-1", "10.0.0.1", DeviceType.Router),
            new(devId2, tenantId, "Switch-1", "10.0.0.2", DeviceType.Switch)
        };

        var histories = new List<DeviceHealthHistory>
        {
            new(Guid.NewGuid(), tenantId, devId1, 45.0, DeviceStatus.Degraded, "High CPU", DateTime.UtcNow),
            new(Guid.NewGuid(), tenantId, devId2, 95.0, DeviceStatus.Online, "Healthy", DateTime.UtcNow)
        };

        _deviceRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(devices);
        _healthHistoryRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(histories);

        var query = new GenerateHealthReportQuery(MaxHealthScore: 50.0, Format: ReportFormat.Json);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.TotalRecords);
    }
}