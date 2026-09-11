using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace Nms.Infrastructure.Observability.Health;

public sealed class RedisHealthCheck : IHealthCheck
{
    private readonly IConnectionMultiplexer? _redis;

    public RedisHealthCheck(IConnectionMultiplexer? redis = null)
    {
        _redis = redis;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        if (_redis == null || !_redis.IsConnected)
        {
            return HealthCheckResult.Degraded("Redis is not connected or not configured.");
        }

        try
        {
            var endpoints = _redis.GetEndPoints();
            if (endpoints.Length == 0)
            {
                return HealthCheckResult.Degraded("No active Redis endpoints discovered.");
            }

            var server = _redis.GetServer(endpoints[0]);
            var latency = await server.PingAsync();

            var data = new Dictionary<string, object>
            {
                { "latency_ms", latency.TotalMilliseconds },
                { "endpoints_count", endpoints.Length }
            };

            return HealthCheckResult.Healthy("Redis cache is responsive.", data);
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Degraded("Redis connection probe encountered an error.", ex);
        }
    }
}