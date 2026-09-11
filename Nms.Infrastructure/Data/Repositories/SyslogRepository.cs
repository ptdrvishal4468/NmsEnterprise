using Microsoft.EntityFrameworkCore;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;

namespace Nms.Infrastructure.Data.Repositories;

public class SyslogRepository : GenericRepository<SyslogMessage, Guid>, ISyslogRepository
{
    public SyslogRepository(NmsDbContext context) : base(context) { }

    public async Task<(IReadOnlyList<SyslogMessage> Items, int TotalCount)> GetSyslogsPagedAsync(
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
        CancellationToken cancellationToken = default)
    {
        IQueryable<SyslogMessage> query = DbSet
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId);

        if (deviceId.HasValue)
        {
            query = query.Where(x => x.DeviceId == deviceId.Value);
        }

        if (severity.HasValue)
        {
            query = query.Where(x => x.Severity == severity.Value);
        }

        if (facility.HasValue)
        {
            query = query.Where(x => x.Facility == facility.Value);
        }

        if (!string.IsNullOrWhiteSpace(sourceIp))
        {
            query = query.Where(x => x.SourceIpAddress == sourceIp.Trim());
        }

        if (!string.IsNullOrWhiteSpace(searchKeyword))
        {
            var keyword = searchKeyword.Trim();
            // Sargable search using EF.Functions.Like without scalar LOWER() conversion
            query = query.Where(x =>
                EF.Functions.Like(x.Message, $"%{keyword}%") ||
                (x.Hostname != null && EF.Functions.Like(x.Hostname, $"%{keyword}%")) ||
                (x.AppTag != null && EF.Functions.Like(x.AppTag, $"%{keyword}%")));
        }

        if (startDateUtc.HasValue)
        {
            query = query.Where(x => x.TimestampUtc >= startDateUtc.Value);
        }

        if (endDateUtc.HasValue)
        {
            query = query.Where(x => x.TimestampUtc <= endDateUtc.Value);
        }

        int totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.TimestampUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}