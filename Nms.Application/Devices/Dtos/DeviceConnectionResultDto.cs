using Nms.Domain.Enums;

namespace Nms.Application.Devices.Dtos;

public sealed record DeviceConnectionResultDto
{
    public required Guid DeviceId { get; init; }
    public required NetworkProtocol Protocol { get; init; }
    public required bool IsConnected { get; init; }
    public required long RoundTripTimeMs { get; init; }
    public string? ErrorMessage { get; init; }
    public required DateTime TestedAtUtc { get; init; }
}