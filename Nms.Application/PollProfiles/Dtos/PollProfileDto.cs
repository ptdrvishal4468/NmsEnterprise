namespace Nms.Application.PollProfiles.Dtos;

public record PollProfileDto(
    Guid Id,
    string Name,
    string? Description,
    int IntervalSeconds,
    int TimeoutSeconds,
    int RetryCount,
    bool IsDefault,
    bool IsEnabled);