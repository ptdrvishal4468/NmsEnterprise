using System.Net;
using System.Net.Http.Json;
using Nms.Application.Discovery.Dtos;
using Nms.Application.Tenants.Commands.CreateTenant;
using Nms.Application.Tenants.Dtos;
using Xunit;

namespace Nms.IntegrationTests.Discovery;

public class DiscoveryEndpointTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public DiscoveryEndpointTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task EnsureTenantContextSetAsync()
    {
        if (!_client.DefaultRequestHeaders.Contains("X-Tenant-Id"))
        {
            var createTenantCommand = new CreateTenantCommand("Discovery Test Tenant");
            var tenantResponse = await _client.PostAsJsonAsync("/api/v1/tenants", createTenantCommand);
            tenantResponse.EnsureSuccessStatusCode();

            var tenant = await tenantResponse.Content.ReadFromJsonAsync<TenantDto>();
            Assert.NotNull(tenant);

            _client.DefaultRequestHeaders.Add("X-Tenant-Id", tenant.Id.ToString());
        }
    }

    [Fact]
    public async Task StartScan_WithInvalidRange_ReturnsBadRequest()
    {
        // Arrange
        await EnsureTenantContextSetAsync();
        var dto = new StartDiscoveryScanDto("Invalid Range Scan", "invalid-range");

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/discovery/scan", dto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetJobById_NonExistent_ReturnsNotFound()
    {
        // Arrange
        await EnsureTenantContextSetAsync();

        // Act
        var response = await _client.GetAsync($"/api/v1/discovery/jobs/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}