namespace Nms.Application.Common.Models;

public record PollJobTask(
    Guid DeviceId,
    Guid TenantId,
    int TimeoutSeconds,
    int RetryCount,
    DateTime ScheduledAtUtc);