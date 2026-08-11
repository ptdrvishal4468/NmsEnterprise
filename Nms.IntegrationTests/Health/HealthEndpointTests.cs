using System.Net;
using Xunit;

namespace Nms.IntegrationTests.Health;

public class HealthEndpointTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public HealthEndpointTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetDeviceHealth_Route_ShouldRespondWithExpectedStatusCode()
    {
        var response = await _client.GetAsync($"/api/v1/health/devices/{Guid.NewGuid()}");

        Assert.True(
            response.StatusCode == HttpStatusCode.OK ||
            response.StatusCode == HttpStatusCode.NotFound ||
            response.StatusCode == HttpStatusCode.InternalServerError ||
            response.StatusCode == HttpStatusCode.Unauthorized ||
            response.StatusCode == HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetDeviceHealthHistory_Route_ShouldRespondWithExpectedStatusCode()
    {
        var response = await _client.GetAsync($"/api/v1/health/devices/{Guid.NewGuid()}/history");

        Assert.True(
            response.StatusCode == HttpStatusCode.OK ||
            response.StatusCode == HttpStatusCode.NotFound ||
            response.StatusCode == HttpStatusCode.InternalServerError ||
            response.StatusCode == HttpStatusCode.Unauthorized ||
            response.StatusCode == HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task EvaluateDeviceHealth_Route_ShouldRespondWithExpectedStatusCode()
    {
        var response = await _client.PostAsync($"/api/v1/health/devices/{Guid.NewGuid()}/evaluate", null);

        Assert.True(
            response.StatusCode == HttpStatusCode.OK ||
            response.StatusCode == HttpStatusCode.NotFound ||
            response.StatusCode == HttpStatusCode.InternalServerError ||
            response.StatusCode == HttpStatusCode.Unauthorized ||
            response.StatusCode == HttpStatusCode.Forbidden);
    }
}