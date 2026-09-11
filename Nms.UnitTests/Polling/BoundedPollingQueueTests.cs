using Nms.Application.Common.Models;
using Nms.Infrastructure.Polling;
using Xunit;

namespace Nms.UnitTests.Polling;

public class BoundedPollingQueueTests
{
    [Fact]
    public async Task EnqueueAndDequeue_MaintainsFifoOrder()
    {
        var queue = new PollingQueue(capacity: 10);
        var job1 = new PollJobTask(Guid.NewGuid(), Guid.NewGuid(), 10, 2, DateTime.UtcNow);
        var job2 = new PollJobTask(Guid.NewGuid(), Guid.NewGuid(), 20, 3, DateTime.UtcNow);

        await queue.EnqueueAsync(job1);
        await queue.EnqueueAsync(job2);

        Assert.Equal(2, queue.Count);

        var dequeued1 = await queue.DequeueAsync();
        var dequeued2 = await queue.DequeueAsync();

        Assert.Equal(job1.DeviceId, dequeued1.DeviceId);
        Assert.Equal(job2.DeviceId, dequeued2.DeviceId);
        Assert.Equal(0, queue.Count);
    }

    [Fact]
    public void TryEnqueue_WhenQueueIsFull_ReturnsFalse()
    {
        var queue = new PollingQueue(capacity: 2);
        var job1 = new PollJobTask(Guid.NewGuid(), Guid.NewGuid(), 10, 2, DateTime.UtcNow);
        var job2 = new PollJobTask(Guid.NewGuid(), Guid.NewGuid(), 10, 2, DateTime.UtcNow);
        var job3 = new PollJobTask(Guid.NewGuid(), Guid.NewGuid(), 10, 2, DateTime.UtcNow);

        Assert.True(queue.TryEnqueue(job1));
        Assert.True(queue.TryEnqueue(job2));
        Assert.False(queue.TryEnqueue(job3));
        Assert.Equal(2, queue.Count);
        Assert.Equal(2, queue.Capacity);
    }

    [Fact]
    public async Task EnqueueAsync_WhenQueueFull_WaitsAndCancelsGracefully()
    {
        var queue = new PollingQueue(capacity: 1);
        var job1 = new PollJobTask(Guid.NewGuid(), Guid.NewGuid(), 10, 2, DateTime.UtcNow);
        var job2 = new PollJobTask(Guid.NewGuid(), Guid.NewGuid(), 10, 2, DateTime.UtcNow);

        await queue.EnqueueAsync(job1);

        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(50));
        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
        {
            await queue.EnqueueAsync(job2, cts.Token);
        });
    }

    [Fact]
    public async Task ConcurrentProducersAndConsumers_HandlesThroughputWithoutLoss()
    {
        const int totalItems = 1000;
        var queue = new PollingQueue(capacity: 100);
        var consumedItems = new List<Guid>();
        var lockObj = new object();

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

        // Producer task
        var producer = Task.Run(async () =>
        {
            for (int i = 0; i < totalItems; i++)
            {
                await queue.EnqueueAsync(new PollJobTask(Guid.NewGuid(), Guid.NewGuid(), 10, 1, DateTime.UtcNow), cts.Token);
            }
        }, cts.Token);

        // Consumer task
        var consumer = Task.Run(async () =>
        {
            while (consumedItems.Count < totalItems && !cts.Token.IsCancellationRequested)
            {
                var item = await queue.DequeueAsync(cts.Token);
                lock (lockObj)
                {
                    consumedItems.Add(item.DeviceId);
                }
            }
        }, cts.Token);

        await Task.WhenAll(producer, consumer);

        Assert.Equal(totalItems, consumedItems.Count);
        Assert.Equal(0, queue.Count);
    }
}