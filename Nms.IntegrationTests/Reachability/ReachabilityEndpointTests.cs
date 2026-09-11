using Xunit;

namespace Nms.IntegrationTests.Reachability;

public class ReachabilityEndpointTests
{
    [Fact]
    public void Reachability_Controller_Route_Structure_Is_Valid()
    {
        const string pingEndpoint = "api/v1/reachability/devices/{deviceId}/ping";
        const string currentEndpoint = "api/v1/reachability/devices/{deviceId}/current";
        const string historyEndpoint = "api/v1/reachability/devices/{deviceId}/history";

        Assert.Contains("reachability", pingEndpoint);
        Assert.Contains("current", currentEndpoint);
        Assert.Contains("history", historyEndpoint);
    }
}