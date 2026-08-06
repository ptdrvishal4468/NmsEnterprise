using Microsoft.Extensions.Logging.Abstractions;
using Nms.Domain.Enums;
using Nms.Domain.Models;
using Nms.Infrastructure.Snmp;
using Xunit;

namespace Nms.UnitTests.Snmp;

public class SnmpClientTests
{
    [Fact]
    public void SnmpClientFactory_ShouldCreateV2cClient()
    {
        var factory = new SnmpClientFactory(NullLoggerFactory.Instance);
        var paramsV2 = ConnectionParameters.ForSnmpV2c("127.0.0.1", 161, "public");

        using var client = factory.CreateClient(paramsV2);

        Assert.NotNull(client);
    }

    [Fact]
    public async Task SnmpClient_ShouldThrowObjectDisposedException_WhenCalledAfterDispose()
    {
        var factory = new SnmpClientFactory(NullLoggerFactory.Instance);
        var paramsV2 = ConnectionParameters.ForSnmpV2c("127.0.0.1", 161, "public");

        var client = factory.CreateClient(paramsV2);
        client.Dispose();

        await Assert.ThrowsAsync<ObjectDisposedException>(() =>
            client.GetAsync(Nms.Domain.ValueObjects.Oid.From(".1.3.6.1.2.1.1.1.0")));
    }
}