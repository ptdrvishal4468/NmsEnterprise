using Nms.Domain.Entities;
using Nms.Domain.Enums;

namespace Nms.Domain.Interfaces;

public interface ISyslogRepository : IGenericRepository<SyslogMessage, Guid>
{
    Task<(IReadOnlyList<SyslogMessage> Items, int TotalCount)> GetSyslogsPagedAsync(
        Guid tenantId,
        Guid? deviceId = null,
        SyslogSeverity? severity = null,
        SyslogFacility? facility = null,
        string? sourceIp = null,
        string? searchKeyword = null,
        DateTime? startDateUtc = null,
        DateTime? endDateUtc = null,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);
}