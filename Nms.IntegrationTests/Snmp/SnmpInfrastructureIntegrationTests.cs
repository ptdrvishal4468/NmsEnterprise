using Microsoft.Extensions.Logging.Abstractions;
using Nms.Domain.ValueObjects;
using Nms.Infrastructure.Snmp;
using Nms.Infrastructure.Snmp.Exceptions;
using Xunit;

namespace Nms.IntegrationTests.Snmp;

public class SnmpInfrastructureIntegrationTests
{
    [Fact]
    public async Task SnmpClient_ShouldThrowTimeoutException_WhenTargetUnreachable()
    {
        var factory = new SnmpClientFactory(NullLoggerFactory.Instance);
        var credentials = new SnmpV2Credentials("public");

        // Non-routable IP address to force timeout execution
        using var client = factory.CreateV2cClient("192.0.2.1", 161, credentials, TimeSpan.FromMilliseconds(500));

        await Assert.ThrowsAsync<SnmpTimeoutException>(() =>
            client.GetAsync(Oid.From(".1.3.6.1.2.1.1.1.0")));
    }
}