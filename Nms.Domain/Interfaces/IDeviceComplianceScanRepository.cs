using Nms.Domain.Entities;
using Nms.Domain.Enums;

namespace Nms.Domain.Interfaces;

/// <summary>
/// Repository contract for managing device compliance scans and evaluating security postures.
/// </summary>
public interface IDeviceComplianceScanRepository : IGenericRepository<DeviceComplianceScan, Guid>
{
    Task<DeviceComplianceScan?> GetScanWithResultsAsync(Guid scanId, CancellationToken cancellationToken = default);
    Task<DeviceComplianceScan?> GetLatestScanForDeviceAsync(Guid deviceId, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<DeviceComplianceScan> Items, int TotalCount)> GetPagedAsync(
        int pageIndex,
        int pageSize,
        Guid? deviceId,
        ComplianceStatus? overallStatus,
        DateTime? fromDateUtc,
        DateTime? toDateUtc,
        CancellationToken cancellationToken = default);
    Task<(int TotalScannedDevices, int CompliantDevices, int NonCompliantDevices, int WarningDevices, int UnableToEvaluateDevices)> GetPostureSummaryAsync(CancellationToken cancellationToken = default);
}