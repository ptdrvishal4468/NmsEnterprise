using Nms.Domain.Enums;

namespace Nms.Domain.Models;

public sealed record ConnectionParameters
{
    public required string HostOrIp { get; init; }
    public required int Port { get; init; }
    public required NetworkProtocol Protocol { get; init; }
    public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(3);
    public string? CommunityString { get; init; }
    public string? Username { get; init; }
    public string? Password { get; init; }

    public static ConnectionParameters ForIcmp(string hostOrIp, TimeSpan? timeout = null) => new()
    {
        HostOrIp = hostOrIp,
        Port = 0,
        Protocol = NetworkProtocol.Icmp,
        Timeout = timeout ?? TimeSpan.FromSeconds(3)
    };

    public static ConnectionParameters ForSnmpV2c(string hostOrIp, int port, string communityString, TimeSpan? timeout = null) => new()
    {
        HostOrIp = hostOrIp,
        Port = port,
        Protocol = NetworkProtocol.SnmpV2c,
        CommunityString = communityString,
        Timeout = timeout ?? TimeSpan.FromSeconds(3)
    };
}