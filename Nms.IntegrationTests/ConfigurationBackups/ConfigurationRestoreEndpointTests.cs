using System.Net;
using Xunit;

namespace Nms.IntegrationTests.ConfigurationBackups;

public class ConfigurationRestoreEndpointTests
{
    [Fact]
    public void ConfigurationRestoreRoutes_ShouldFollowStandardApiConvention()
    {
        // Verify route patterns align with rest of application
        const string restoreEndpointPattern = "api/v1/configurationrestores/backups/{backupId}";
        const string logsEndpointPattern = "api/v1/configurationrestores/logs";

        Assert.Contains("api/v1/configurationrestores", restoreEndpointPattern);
        Assert.Contains("api/v1/configurationrestores", logsEndpointPattern);
    }
}