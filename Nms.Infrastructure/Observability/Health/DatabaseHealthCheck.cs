using Microsoft.Extensions.Diagnostics.HealthChecks;
using Nms.Infrastructure.Data;

namespace Nms.Infrastructure.Observability.Health;

public sealed class DatabaseHealthCheck : IHealthCheck
{
    private readonly NmsDbContext _dbContext;

    public DatabaseHealthCheck(NmsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var canConnect = await _dbContext.Database.CanConnectAsync(cancellationToken);

            return canConnect
                ? HealthCheckResult.Healthy("Database connection is healthy.")
                : HealthCheckResult.Unhealthy("Unable to establish connection to the primary database.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Database health check failed.", ex);
        }
    }
}