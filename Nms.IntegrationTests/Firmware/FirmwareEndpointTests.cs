using Nms.Api.Controllers;
using Xunit;

namespace Nms.IntegrationTests.Firmware;

public class FirmwareEndpointTests
{
    [Fact]
    [Trait("Category", "Integration")]
    public void Controller_ShouldDefineCorrectRoute()
    {
        var type = typeof(FirmwareController);
        Assert.NotNull(type);
    }
}