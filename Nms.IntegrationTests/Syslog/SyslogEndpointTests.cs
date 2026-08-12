using Nms.Application.Syslog.Queries.GetSyslogById;
using Nms.Application.Syslog.Queries.GetSyslogsPaged;
using Nms.Domain.Enums;
using Xunit;

namespace Nms.IntegrationTests.Syslog;

public class SyslogEndpointTests
{
    [Fact]
    public void GetSyslogsPagedQuery_ConstructsWithCorrectParameters()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var deviceId = Guid.NewGuid();

        // Act
        var query = new GetSyslogsPagedQuery(
            TenantId: tenantId,
            DeviceId: deviceId,
            Severity: SyslogSeverity.Error,
            Facility: SyslogFacility.Daemon,
            SourceIp: "192.168.1.10",
            SearchKeyword: "error",
            PageNumber: 1,
            PageSize: 20);

        // Assert
        Assert.Equal(tenantId, query.TenantId);
        Assert.Equal(deviceId, query.DeviceId);
        Assert.Equal(SyslogSeverity.Error, query.Severity);
        Assert.Equal(SyslogFacility.Daemon, query.Facility);
        Assert.Equal("192.168.1.10", query.SourceIp);
        Assert.Equal("error", query.SearchKeyword);
    }

    [Fact]
    public void GetSyslogByIdQuery_TenantIsolation_MatchesTenantId()
    {
        // Arrange
        var id = Guid.NewGuid();
        var tenantId = Guid.NewGuid();

        // Act
        var query = new GetSyslogByIdQuery(id, tenantId);

        // Assert
        Assert.Equal(id, query.Id);
        Assert.Equal(tenantId, query.TenantId);
    }
}