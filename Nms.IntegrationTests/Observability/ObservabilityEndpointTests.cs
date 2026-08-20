using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Nms.IntegrationTests.Observability;

public sealed class ObservabilityEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ObservabilityEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task HealthLive_ShouldReturnOk()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/health/live");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Healthy", content);
    }

    [Fact]
    public async Task HealthReady_ShouldExecuteReadinessProbes()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/health/ready");

        // In test environment without SQL Server/Redis containers active, readiness returns 200 or 503 (Degraded/Unhealthy)
        Assert.True(
            response.StatusCode == HttpStatusCode.OK ||
            response.StatusCode == HttpStatusCode.ServiceUnavailable);

        var content = await response.Content.ReadAsStringAsync();
        Assert.False(string.IsNullOrWhiteSpace(content));
    }
}