using Nms.Application.Common.Models;

namespace Nms.Application.Common.Interfaces;

public interface IPollingQueue
{
    ValueTask EnqueueAsync(PollJobTask job, CancellationToken cancellationToken = default);
    ValueTask<PollJobTask> DequeueAsync(CancellationToken cancellationToken = default);
    int Count { get; }
}