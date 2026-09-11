using Microsoft.EntityFrameworkCore;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;

namespace Nms.Infrastructure.Data.Repositories;

public class DeviceComplianceScanRepository : GenericRepository<DeviceComplianceScan, Guid>, IDeviceComplianceScanRepository
{
    public DeviceComplianceScanRepository(NmsDbContext context) : base(context) { }

    public async Task<DeviceComplianceScan?> GetScanWithResultsAsync(Guid scanId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(s => s.Device)
            .Include(s => s.Results)
                .ThenInclude(r => r.Policy)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == scanId, cancellationToken);
    }

    public async Task<DeviceComplianceScan?> GetLatestScanForDeviceAsync(Guid deviceId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(s => s.Device)
            .Include(s => s.Results)
                .ThenInclude(r => r.Policy)
            .AsNoTracking()
            .Where(s => s.DeviceId == deviceId)
            .OrderByDescending(s => s.ScannedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<DeviceComplianceScan> Items, int TotalCount)> GetPagedAsync(
        int pageIndex,
        int pageSize,
        Guid? deviceId,
        ComplianceStatus? overallStatus,
        DateTime? fromDateUtc,
        DateTime? toDateUtc,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .Include(s => s.Device)
            .AsNoTracking()
            .AsQueryable();

        if (deviceId.HasValue && deviceId.Value != Guid.Empty)
        {
            query = query.Where(s => s.DeviceId == deviceId.Value);
        }

        if (overallStatus.HasValue)
        {
            query = query.Where(s => s.OverallStatus == overallStatus.Value);
        }

        if (fromDateUtc.HasValue)
        {
            query = query.Where(s => s.ScannedAtUtc >= fromDateUtc.Value);
        }

        if (toDateUtc.HasValue)
        {
            query = query.Where(s => s.ScannedAtUtc <= toDateUtc.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(s => s.ScannedAtUtc)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<(int TotalScannedDevices, int CompliantDevices, int NonCompliantDevices, int WarningDevices, int UnableToEvaluateDevices)> GetPostureSummaryAsync(CancellationToken cancellationToken = default)
    {
        // Obtain latest scan for each device
        var latestScans = await DbSet
            .AsNoTracking()
            .GroupBy(s => s.DeviceId)
            .Select(g => g.OrderByDescending(s => s.ScannedAtUtc).First())
            .ToListAsync(cancellationToken);

        int total = latestScans.Count;
        int compliant = latestScans.Count(s => s.OverallStatus == ComplianceStatus.Compliant);
        int nonCompliant = latestScans.Count(s => s.OverallStatus == ComplianceStatus.NonCompliant);
        int warning = latestScans.Count(s => s.OverallStatus == ComplianceStatus.Warning);
        int unable = latestScans.Count(s => s.OverallStatus == ComplianceStatus.UnableToEvaluate || s.OverallStatus == ComplianceStatus.NotApplicable);

        return (total, compliant, nonCompliant, warning, unable);
    }
}