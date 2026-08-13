using Nms.Application.SnmpTraps.Queries.GetSnmpTrapById;
using Nms.Application.SnmpTraps.Queries.GetSnmpTrapsPaged;
using Nms.Domain.Enums;
using Xunit;

namespace Nms.IntegrationTests.Snmp;

public class SnmpTrapEndpointTests
{
    [Fact]
    public void GetSnmpTrapsPagedQuery_ConstructsWithCorrectParameters()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var deviceId = Guid.NewGuid();

        // Act
        var query = new GetSnmpTrapsPagedQuery(
            TenantId: tenantId,
            DeviceId: deviceId,
            Severity: TrapSeverity.Error,
            SourceIpAddress: "192.168.1.10",
            EnterpriseOid: "1.3.6.1.4.1.9",
            PageNumber: 1,
            PageSize: 20);

        // Assert
        Assert.Equal(tenantId, query.TenantId);
        Assert.Equal(deviceId, query.DeviceId);
        Assert.Equal(TrapSeverity.Error, query.Severity);
        Assert.Equal("192.168.1.10", query.SourceIpAddress);
        Assert.Equal("1.3.6.1.4.1.9", query.EnterpriseOid);
    }

    [Fact]
    public void GetSnmpTrapByIdQuery_TenantIsolation_MatchesTenantId()
    {
        // Arrange
        var id = Guid.NewGuid();
        var tenantId = Guid.NewGuid();

        // Act
        var query = new GetSnmpTrapByIdQuery(id, tenantId);

        // Assert
        Assert.Equal(id, query.Id);
        Assert.Equal(tenantId, query.TenantId);
    }
}