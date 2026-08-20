using Nms.Infrastructure.Observability.Metrics;
using Xunit;

namespace Nms.UnitTests.Observability;

public sealed class NmsMetricsTests
{
    [Fact]
    public void RecordPollingJobEnqueued_ShouldExecuteWithoutExceptions()
    {
        var exception = Record.Exception(() => NmsMetrics.RecordPollingJobEnqueued("snmp"));
        Assert.Null(exception);
    }

    [Theory]
    [InlineData("snmp", true, 42.5)]
    [InlineData("icmp", false, 120.0)]
    public void RecordPollingJobProcessed_ShouldExecuteWithoutExceptions(string jobType, bool success, double durationMs)
    {
        var exception = Record.Exception(() => NmsMetrics.RecordPollingJobProcessed(jobType, success, durationMs));
        Assert.Null(exception);
    }

    [Fact]
    public void RecordAlertEvaluation_ShouldExecuteWithoutExceptions()
    {
        var exception = Record.Exception(() => NmsMetrics.RecordAlertEvaluation(5));
        Assert.Null(exception);
    }

    [Fact]
    public void RecordAlertTriggered_ShouldExecuteWithoutExceptions()
    {
        var exception = Record.Exception(() => NmsMetrics.RecordAlertTriggered("Critical"));
        Assert.Null(exception);
    }

    [Theory]
    [InlineData(true, 12.3)]
    [InlineData(false, 0.0)]
    public void RecordReachabilityPing_ShouldExecuteWithoutExceptions(bool success, double latencyMs)
    {
        var exception = Record.Exception(() => NmsMetrics.RecordReachabilityPing(success, latencyMs));
        Assert.Null(exception);
    }
}