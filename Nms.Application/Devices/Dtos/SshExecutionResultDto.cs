namespace Nms.Application.Devices.Dtos;

public sealed record SshExecutionResultDto(
    bool IsSuccess,
    int ExitCode,
    string Output,
    string ErrorOutput,
    long DurationMs
);