namespace Nms.Application.PollProfiles.Dtos;

public record CreatePollProfileDto(
    string Name,
    string? Description,
    int IntervalSeconds,
    int TimeoutSeconds,
    int RetryCount,
    bool IsDefault);