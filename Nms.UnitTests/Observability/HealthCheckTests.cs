using Microsoft.Extensions.Diagnostics.HealthChecks;
using Nms.Infrastructure.Observability.Health;
using Xunit;

namespace Nms.UnitTests.Observability;

public sealed class HealthCheckTests
{
    [Fact]
    public async Task RedisHealthCheck_WhenMultiplexerIsNull_ShouldReturnDegraded()
    {
        var healthCheck = new RedisHealthCheck(redis: null);
        var context = new HealthCheckContext();

        var result = await healthCheck.CheckHealthAsync(context, CancellationToken.None);

        Assert.Equal(HealthStatus.Degraded, result.Status);
        Assert.Contains("not connected or not configured", result.Description);
    }
}