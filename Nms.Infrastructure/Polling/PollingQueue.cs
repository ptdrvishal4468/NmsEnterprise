using System.Threading.Channels;
using Nms.Application.Common.Interfaces;
using Nms.Application.Common.Models;

namespace Nms.Infrastructure.Polling;

public class PollingQueue : IPollingQueue
{
    private readonly Channel<PollJobTask> _channel;

    public PollingQueue(int capacity = 10000)
    {
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

    public async ValueTask<PollJobTask> DequeueAsync(CancellationToken cancellationToken = default)
    {
        return await _channel.Reader.ReadAsync(cancellationToken);
    }

    public int Count => _channel.Reader.Count;
}