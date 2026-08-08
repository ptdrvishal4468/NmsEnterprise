using Nms.Application.Common.Models;
using Nms.Infrastructure.Polling;
using Xunit;

namespace Nms.UnitTests.Polling;

public class PollingQueueTests
{
    [Fact]
    public async Task EnqueueAndDequeue_ShouldMaintainFifoOrder()
    {
        // Arrange
        var queue = new PollingQueue(capacity: 100);
        var job1 = new PollJobTask(Guid.NewGuid(), Guid.NewGuid(), 5, 3, DateTime.UtcNow);
        var job2 = new PollJobTask(Guid.NewGuid(), Guid.NewGuid(), 10, 2, DateTime.UtcNow);

        // Act
        await queue.EnqueueAsync(job1);
        await queue.EnqueueAsync(job2);

        var dequeued1 = await queue.DequeueAsync();
        var dequeued2 = await queue.DequeueAsync();

        // Assert
        Assert.Equal(job1.DeviceId, dequeued1.DeviceId);
        Assert.Equal(job2.DeviceId, dequeued2.DeviceId);
    }

    [Fact]
    public async Task Count_ShouldReflectQueuedItems()
    {
        // Arrange
        var queue = new PollingQueue(capacity: 10);
        var job = new PollJobTask(Guid.NewGuid(), Guid.NewGuid(), 5, 3, DateTime.UtcNow);

        // Act
        Assert.Equal(0, queue.Count);
        await queue.EnqueueAsync(job);

        // Assert
        Assert.Equal(1, queue.Count);

        await queue.DequeueAsync();
        Assert.Equal(0, queue.Count);
    }
}