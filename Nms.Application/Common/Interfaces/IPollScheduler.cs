namespace Nms.Application.Common.Interfaces;

public interface IPollScheduler
{
    Task SchedulePendingPollsAsync(CancellationToken cancellationToken = default);
}