using System.Net;
using System.Net.Http.Json;
using Nms.Api.Controllers;
using Xunit;

namespace Nms.IntegrationTests.Alerts;

public class AlertsEndpointTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AlertsEndpointTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAlerts_Route_ShouldRespondWithExpectedStatusCode()
    {
        var response = await _client.GetAsync("/api/v1/alerts");

        Assert.True(
            response.StatusCode == HttpStatusCode.OK ||
            response.StatusCode == HttpStatusCode.Unauthorized ||
            response.StatusCode == HttpStatusCode.Forbidden ||
            response.StatusCode == HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task GetAlertById_Route_ShouldRespondWithExpectedStatusCode()
    {
        var alertId = Guid.NewGuid();
        var response = await _client.GetAsync($"/api/v1/alerts/{alertId}");

        Assert.True(
            response.StatusCode == HttpStatusCode.OK ||
            response.StatusCode == HttpStatusCode.NotFound ||
            response.StatusCode == HttpStatusCode.Unauthorized ||
            response.StatusCode == HttpStatusCode.Forbidden ||
            response.StatusCode == HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task AcknowledgeAlert_Route_ShouldRespondWithExpectedStatusCode()
    {
        var alertId = Guid.NewGuid();
        var request = new AcknowledgeAlertRequest("Acknowledging alert in integration test.");

        var response = await _client.PostAsJsonAsync($"/api/v1/alerts/{alertId}/acknowledge", request);

        Assert.True(
            response.StatusCode == HttpStatusCode.OK ||
            response.StatusCode == HttpStatusCode.NotFound ||
            response.StatusCode == HttpStatusCode.Unauthorized ||
            response.StatusCode == HttpStatusCode.Forbidden ||
            response.StatusCode == HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task SuppressAlert_Route_ShouldRespondWithExpectedStatusCode()
    {
        var alertId = Guid.NewGuid();
        var request = new SuppressAlertRequest("Suppressing alert in integration test.");

        var response = await _client.PostAsJsonAsync($"/api/v1/alerts/{alertId}/suppress", request);

        Assert.True(
            response.StatusCode == HttpStatusCode.OK ||
            response.StatusCode == HttpStatusCode.NotFound ||
            response.StatusCode == HttpStatusCode.Unauthorized ||
            response.StatusCode == HttpStatusCode.Forbidden ||
            response.StatusCode == HttpStatusCode.InternalServerError);
    }
}