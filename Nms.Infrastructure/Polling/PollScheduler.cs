using Nms.Application.Common.Interfaces;
using Nms.Application.Common.Models;
using Nms.Domain.Interfaces;

namespace Nms.Infrastructure.Polling;

public class PollScheduler : IPollScheduler
{
    private readonly IDeviceRepository _deviceRepository;
    private readonly IPollProfileRepository _profileRepository;
    private readonly IPollingQueue _queue;

    public PollScheduler(
        IDeviceRepository deviceRepository,
        IPollProfileRepository profileRepository,
        IPollingQueue queue)
    {
        _deviceRepository = deviceRepository;
        _profileRepository = profileRepository;
        _queue = queue;
    }

    public async Task SchedulePendingPollsAsync(CancellationToken cancellationToken = default)
    {
        var devices = await _deviceRepository.GetAllAsync(cancellationToken);

        foreach (var device in devices)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            int timeoutSeconds = 5;
            int retryCount = 3;

            if (device.PollProfileId.HasValue)
            {
                var profile = await _profileRepository.GetByIdAsync(device.PollProfileId.Value, cancellationToken);
                if (profile != null && !profile.IsEnabled)
                {
                    continue; // Skip polling if profile is explicitly disabled
                }

                if (profile != null)
                {
                    timeoutSeconds = profile.TimeoutSeconds;
                    retryCount = profile.RetryCount;
                }
            }

            var job = new PollJobTask(
                device.Id,
                device.TenantId,
                timeoutSeconds,
                retryCount,
                DateTime.UtcNow);

            await _queue.EnqueueAsync(job, cancellationToken);
        }
    }
}