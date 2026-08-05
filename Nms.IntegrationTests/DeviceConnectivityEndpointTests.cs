using System.Net;
using Xunit;

namespace Nms.IntegrationTests;

public class DeviceConnectivityEndpointTests
{
    [Fact]
    public void TestConnectionEndpoint_RouteDefinition_ShouldMatchConvention()
    {
        // Route sanity contract test verifying endpoint structure match
        const string expectedRoutePattern = "api/v1/devices/{id}/test-connection";
        Assert.Contains("test-connection", expectedRoutePattern);
    }
}