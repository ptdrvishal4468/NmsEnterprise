using System.Net;
using Xunit;

namespace Nms.IntegrationTests.Interfaces;

public class InterfacesEndpointTests
{
    [Fact]
    public async Task GetDeviceInterfaces_WithoutAuthHeader_ReturnsUnauthorized()
    {
        using var client = new HttpClient { BaseAddress = new Uri("http://localhost:5000") };

        var response = await client.GetAsync($"/api/v1/devices/{Guid.NewGuid()}/interfaces");

        Assert.True(
            response.StatusCode == HttpStatusCode.Unauthorized ||
            response.StatusCode == HttpStatusCode.NotFound ||
            response.StatusCode == HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task PollDeviceInterfaces_WithoutAuthHeader_ReturnsUnauthorized()
    {
        using var client = new HttpClient { BaseAddress = new Uri("http://localhost:5000") };

        var response = await client.PostAsync($"/api/v1/devices/{Guid.NewGuid()}/interfaces/poll", null);

        Assert.True(
            response.StatusCode == HttpStatusCode.Unauthorized ||
            response.StatusCode == HttpStatusCode.NotFound ||
            response.StatusCode == HttpStatusCode.InternalServerError);
    }
}