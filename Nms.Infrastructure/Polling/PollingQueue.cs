using System.Threading.Channels;
using Microsoft.Extensions.Options;
using Nms.Application.Common.Interfaces;
using Nms.Application.Common.Models;

namespace Nms.Infrastructure.Polling;

public class PollingQueue : IPollingQueue
{
    private readonly Channel<PollJobTask> _channel;
    private readonly int _capacity;

    public PollingQueue(IOptions<PollingQueueOptions> options)
        : this(options.Value.Capacity)
    {
    }

    public PollingQueue(int capacity = 10000)
    {
        if (capacity < 1)
            throw new ArgumentOutOfRangeException(nameof(capacity));

        _capacity = capacity;
        var options = new BoundedChannelOptions(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = false,
            SingleWriter = false
        };
        _channel = Channel.CreateBounded<PollJobTask>(options);
    }

    public async ValueTask EnqueueAsync(PollJobTask job, CancellationToken cancellationToken = default)
    {
        await _channel.Writer.WriteAsync(job, cancellationToken);
    }

    public bool TryEnqueue(PollJobTask job)
    {
        return _channel.Writer.TryWrite(job);
    }

    public async ValueTask<PollJobTask> DequeueAsync(CancellationToken cancellationToken = default)
    {
        return await _channel.Reader.ReadAsync(cancellationToken);
    }

    public int Count => _channel.Reader.Count;

    public int Capacity => _capacity;
}