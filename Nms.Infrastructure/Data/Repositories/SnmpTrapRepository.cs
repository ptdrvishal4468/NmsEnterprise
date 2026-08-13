using Microsoft.EntityFrameworkCore;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;

namespace Nms.Infrastructure.Data.Repositories;

public sealed class SnmpTrapRepository : GenericRepository<SnmpTrapMessage, Guid>, ISnmpTrapRepository
{
    public SnmpTrapRepository(NmsDbContext context) : base(context)
    {
    }

    public async Task<(IReadOnlyList<SnmpTrapMessage> Items, int TotalCount)> GetSnmpTrapsPagedAsync(
        Guid tenantId,
        Guid? deviceId = null,
        TrapSeverity? severity = null,
        string? sourceIpAddress = null,
        string? enterpriseOid = null,
        DateTime? fromUtc = null,
        DateTime? toUtc = null,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = Context.SnmpTrapMessages.AsNoTracking().Where(x => x.TenantId == tenantId);

        if (deviceId.HasValue)
        {
            query = query.Where(x => x.DeviceId == deviceId.Value);
        }

        if (severity.HasValue)
        {
            query = query.Where(x => x.Severity == severity.Value);
        }

        if (!string.IsNullOrWhiteSpace(sourceIpAddress))
        {
            query = query.Where(x => x.SourceIpAddress.Contains(sourceIpAddress));
        }

        if (!string.IsNullOrWhiteSpace(enterpriseOid))
        {
            query = query.Where(x => x.EnterpriseOid.Contains(enterpriseOid));
        }

        if (fromUtc.HasValue)
        {
            query = query.Where(x => x.TimestampUtc >= fromUtc.Value);
        }

        if (toUtc.HasValue)
        {
            query = query.Where(x => x.TimestampUtc <= toUtc.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.TimestampUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}