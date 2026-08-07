namespace Nms.Domain.Models;

public sealed record SshExecutionResult
{
    public required bool IsSuccess { get; init; }
    public required int ExitCode { get; init; }
    public required string Output { get; init; }
    public required string ErrorOutput { get; init; }
    public required long DurationMs { get; init; }

    public static SshExecutionResult Success(string output, long durationMs, int exitCode = 0) => new()
    {
        IsSuccess = true,
        ExitCode = exitCode,
        Output = output,
        ErrorOutput = string.Empty,
        DurationMs = durationMs
    };

    public static SshExecutionResult Failure(string errorOutput, long durationMs, int exitCode = -1) => new()
    {
        IsSuccess = false,
        ExitCode = exitCode,
        Output = string.Empty,
        ErrorOutput = errorOutput,
        DurationMs = durationMs
    };
}