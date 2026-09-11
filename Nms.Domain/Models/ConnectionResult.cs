using Nms.Domain.Enums;

namespace Nms.Domain.Models;

public sealed record ConnectionResult
{
    public required bool IsConnected { get; init; }
    public required NetworkProtocol Protocol { get; init; }
    public required long RoundTripTimeMs { get; init; }
    public string? ErrorMessage { get; init; }
    public DateTime TestedAtUtc { get; init; } = DateTime.UtcNow;

    public static ConnectionResult Success(NetworkProtocol protocol, long roundTripTimeMs) => new()
    {
        IsConnected = true,
        Protocol = protocol,
        RoundTripTimeMs = roundTripTimeMs,
        ErrorMessage = null
    };

    public static ConnectionResult Failure(NetworkProtocol protocol, string errorMessage) => new()
    {
        IsConnected = false,
        Protocol = protocol,
        RoundTripTimeMs = 0,
        ErrorMessage = errorMessage
    };
}