using System.Net;
using System.Net.Http.Json;
using Nms.Application.Common.Models;
using Nms.Application.Tenants.Commands.CreateTenant;
using Nms.Application.Tenants.Dtos;
using Xunit;

namespace Nms.IntegrationTests;

public class TenantEndpointTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public TenantEndpointTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateAndGetTenant_ShouldReturnCreatedAndOk()
    {
        // Arrange
        var createCommand = new CreateTenantCommand("Enterprise Telecom Org");

        // Act - 1. Create Tenant
        var createResponse = await _client.PostAsJsonAsync("/api/v1/tenants", createCommand);

        // Assert - Creation
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var createdTenant = await createResponse.Content.ReadFromJsonAsync<TenantDto>();
        Assert.NotNull(createdTenant);
        Assert.Equal("Enterprise Telecom Org", createdTenant.Name);
        Assert.True(createdTenant.IsActive);

        // Act - 2. Get Tenant By ID
        var getResponse = await _client.GetAsync($"/api/v1/tenants/{createdTenant.Id}");

        // Assert - Retrieval
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var fetchedTenant = await getResponse.Content.ReadFromJsonAsync<TenantDto>();
        Assert.NotNull(fetchedTenant);
        Assert.Equal(createdTenant.Id, fetchedTenant.Id);
        Assert.Equal("Enterprise Telecom Org", fetchedTenant.Name);
    }

    [Fact]
    public async Task GetTenants_ShouldReturnPagedResult()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/tenants?pageNumber=1&pageSize=10");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<TenantDto>>();
        Assert.NotNull(result);
    }
}