using System.Net;
using System.Net.Http.Json;
using Nms.Application.Common.Models;
using Nms.Application.PollProfiles.Dtos;
using Nms.Application.Tenants.Commands.CreateTenant;
using Nms.Application.Tenants.Dtos;
using Xunit;

namespace Nms.IntegrationTests.Polling;

public class PollProfilesEndpointTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public PollProfilesEndpointTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task EnsureTenantContextSetAsync()
    {
        if (!_client.DefaultRequestHeaders.Contains("X-Tenant-Id"))
        {
            var createTenantCommand = new CreateTenantCommand("PollProfile Test Tenant");
            var tenantResponse = await _client.PostAsJsonAsync("/api/v1/tenants", createTenantCommand);
            tenantResponse.EnsureSuccessStatusCode();

            var tenant = await tenantResponse.Content.ReadFromJsonAsync<TenantDto>();
            Assert.NotNull(tenant);

            _client.DefaultRequestHeaders.Add("X-Tenant-Id", tenant.Id.ToString());
        }
    }

    [Fact]
    public async Task CreateAndGetPollProfile_ShouldReturnCreatedAndOk()
    {
        // Arrange
        await EnsureTenantContextSetAsync();

        var createDto = new CreatePollProfileDto(
            Name: "High Frequency Profile",
            Description: "For critical core switches",
            IntervalSeconds: 30,
            TimeoutSeconds: 5,
            RetryCount: 3,
            IsDefault: false);

        // Act - 1. Create Profile
        var createResponse = await _client.PostAsJsonAsync("/api/v1/poll-profiles", createDto);

        // Assert - Creation
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var createdId = await createResponse.Content.ReadFromJsonAsync<Guid>();
        Assert.NotEqual(Guid.Empty, createdId);

        // Act - 2. Get Profile By ID
        var getResponse = await _client.GetAsync($"/api/v1/poll-profiles/{createdId}");

        // Assert - Retrieval
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var profile = await getResponse.Content.ReadFromJsonAsync<PollProfileDto>();
        Assert.NotNull(profile);
        Assert.Equal("High Frequency Profile", profile.Name);
        Assert.Equal(30, profile.IntervalSeconds);
    }

    [Fact]
    public async Task GetPollProfiles_ShouldReturnPagedResult()
    {
        // Arrange
        await EnsureTenantContextSetAsync();

        // Act
        var response = await _client.GetAsync("/api/v1/poll-profiles?pageNumber=1&pageSize=10");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<PollProfileDto>>();
        Assert.NotNull(result);
    }

    [Fact]
    public async Task CreatePollProfile_WithInvalidInterval_ShouldReturnBadRequest()
    {
        // Arrange
        await EnsureTenantContextSetAsync();

        var invalidDto = new CreatePollProfileDto(
            Name: "Invalid Profile",
            Description: "Interval below minimum allowed",
            IntervalSeconds: 2, // Violates FluentValidation rule (>= 10)
            TimeoutSeconds: 5,
            RetryCount: 3,
            IsDefault: false);

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/poll-profiles", invalidDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}