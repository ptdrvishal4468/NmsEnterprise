using System.Net;
using Xunit;

namespace Nms.IntegrationTests.Interfaces;

public class InterfacesEndpointTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public InterfacesEndpointTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetDeviceInterfaces_Route_ShouldRespondSuccessfully()
    {
        var response = await _client.GetAsync($"/api/v1/devices/{Guid.NewGuid()}/interfaces");

        Assert.True(
            response.StatusCode == HttpStatusCode.OK ||
            response.StatusCode == HttpStatusCode.Unauthorized ||
            response.StatusCode == HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task PollDeviceInterfaces_NonExistentDevice_ReturnsNotFoundOrInternalError()
    {
        var response = await _client.PostAsync($"/api/v1/devices/{Guid.NewGuid()}/interfaces/poll", null);

        Assert.True(
            response.StatusCode == HttpStatusCode.NotFound ||
            response.StatusCode == HttpStatusCode.InternalServerError ||
            response.StatusCode == HttpStatusCode.Unauthorized);
    }
}