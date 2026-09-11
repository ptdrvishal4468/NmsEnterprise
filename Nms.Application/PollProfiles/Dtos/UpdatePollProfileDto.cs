namespace Nms.Application.PollProfiles.Dtos;

public record UpdatePollProfileDto(
    string Name,
    string? Description,
    int IntervalSeconds,
    int TimeoutSeconds,
    int RetryCount);