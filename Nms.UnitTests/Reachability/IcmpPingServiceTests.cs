using Nms.Domain.Enums;
using Nms.Domain.Models;
using Xunit;

namespace Nms.UnitTests.Reachability;

public class IcmpPingServiceTests
{
    [Fact]
    public void PingResult_CreateFailure_Should_Set_100Percent_PacketLoss()
    {
        var failure = PingResult.CreateFailure(ReachabilityStatus.Timeout, 4);

        Assert.Equal(ReachabilityStatus.Timeout, failure.Status);
        Assert.Equal(4, failure.PacketsSent);
        Assert.Equal(0, failure.PacketsReceived);
        Assert.Equal(100.0, failure.PacketLossPercentage);
        Assert.Equal(0, failure.AvgLatencyMs);
    }

    [Fact]
    public void PingResult_Constructor_Should_Assign_Metrics_Correctly()
    {
        var result = new PingResult(
            ReachabilityStatus.Online,
            minLatencyMs: 5.0,
            maxLatencyMs: 20.0,
            avgLatencyMs: 12.5,
            currentLatencyMs: 10.0,
            packetsSent: 4,
            packetsReceived: 3,
            packetLossPercentage: 25.0);

        Assert.Equal(ReachabilityStatus.Online, result.Status);
        Assert.Equal(5.0, result.MinLatencyMs);
        Assert.Equal(20.0, result.MaxLatencyMs);
        Assert.Equal(12.5, result.AvgLatencyMs);
        Assert.Equal(10.0, result.CurrentLatencyMs);
        Assert.Equal(25.0, result.PacketLossPercentage);
    }
}