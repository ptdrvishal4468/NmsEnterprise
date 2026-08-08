using Moq;
using Nms.Application.Common.Interfaces;
using Nms.Application.Common.Models;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;
using Nms.Infrastructure.Polling;
using Xunit;

namespace Nms.UnitTests.Polling;

public class PollSchedulerTests
{
    private readonly Mock<IDeviceRepository> _deviceRepoMock;
    private readonly Mock<IPollProfileRepository> _profileRepoMock;
    private readonly Mock<IPollingQueue> _queueMock;
    private readonly PollScheduler _scheduler;

    public PollSchedulerTests()
    {
        _deviceRepoMock = new Mock<IDeviceRepository>();
        _profileRepoMock = new Mock<IPollProfileRepository>();
        _queueMock = new Mock<IPollingQueue>();

        _scheduler = new PollScheduler(
            _deviceRepoMock.Object,
            _profileRepoMock.Object,
            _queueMock.Object);
    }

    [Fact]
    public async Task SchedulePendingPollsAsync_ShouldEnqueueJobsForDevices()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var device1 = new Device(Guid.NewGuid(), tenantId, "Router-1", "192.168.1.1", DeviceType.Router);
        var device2 = new Device(Guid.NewGuid(), tenantId, "Switch-1", "192.168.1.2", DeviceType.Switch);

        _deviceRepoMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Device> { device1, device2 });

        // Act
        await _scheduler.SchedulePendingPollsAsync(CancellationToken.None);

        // Assert
        _queueMock.Verify(q => q.EnqueueAsync(It.IsAny<PollJobTask>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }
}