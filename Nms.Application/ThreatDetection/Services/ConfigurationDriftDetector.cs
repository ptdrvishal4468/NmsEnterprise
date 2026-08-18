using System.Text.Json;
using Nms.Application.Common.Interfaces;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;

namespace Nms.Application.ThreatDetection.Services;

public class ConfigurationDriftDetector : IConfigurationDriftDetector
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfigurationStorageService _storageService;
    private readonly ITenantContext _tenantContext;

    public ConfigurationDriftDetector(
        IUnitOfWork unitOfWork,
        IConfigurationStorageService storageService,
        ITenantContext tenantContext)
    {
        _unitOfWork = unitOfWork;
        _storageService = storageService;
        _tenantContext = tenantContext;
    }

    public async Task<ConfigurationDriftRecord> AnalyzeDeviceDriftAsync(Guid deviceId, CancellationToken cancellationToken = default)
    {
        var (backups, _) = await _unitOfWork.ConfigurationBackups.GetBackupsPagedAsync(
            tenantId: _tenantContext.TenantId,
            deviceId: deviceId,
            status: BackupStatus.Success,
            triggerType: null,
            pageNumber: 1,
            pageSize: 50,
            cancellationToken: cancellationToken);

        var baseline = backups.OrderBy(b => b.VersionNumber).FirstOrDefault();
        var current = backups.OrderByDescending(b => b.VersionNumber).FirstOrDefault();

        if (baseline == null || current == null || baseline.Id == current.Id)
        {
            var cleanRecord = new ConfigurationDriftRecord
            {
                TenantId = _tenantContext.TenantId,
                DeviceId = deviceId,
                BaselineBackupId = baseline?.Id ?? Guid.Empty,
                CurrentBackupId = current?.Id ?? Guid.Empty,
                HasDrift = false,
                Severity = ThreatSeverity.Low,
                DetectedAtUtc = DateTime.UtcNow
            };
            return cleanRecord;
        }

        var baselineContent = (await _storageService.GetBackupFileContentAsync(baseline.StoragePath, cancellationToken)) ?? string.Empty;
        var currentContent = (await _storageService.GetBackupFileContentAsync(current.StoragePath, cancellationToken)) ?? string.Empty;

        var baselineLines = baselineContent
            .Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(l => l.Trim())
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .Distinct(StringComparer.Ordinal)
            .ToList();

        var currentLines = currentContent
            .Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(l => l.Trim())
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .Distinct(StringComparer.Ordinal)
            .ToList();

        var baselineSet = new HashSet<string>(baselineLines, StringComparer.Ordinal);
        var currentSet = new HashSet<string>(currentLines, StringComparer.Ordinal);

        var added = currentLines
            .Where(line => !baselineSet.Contains(line))
            .Distinct(StringComparer.Ordinal)
            .ToList();
        var removed = baselineLines
            .Where(line => !currentSet.Contains(line))
            .Distinct(StringComparer.Ordinal)
            .ToList();
        var hasDrift = added.Count > 0 || removed.Count > 0;

        var diffSummary = new
        {
            Added = added.Take(50).ToList(),
            Removed = removed.Take(50).ToList(),
            TotalAdded = added.Count,
            TotalRemoved = removed.Count
        };

        var driftRecord = new ConfigurationDriftRecord
        {
            TenantId = _tenantContext.TenantId,
            DeviceId = deviceId,
            BaselineBackupId = baseline.Id,
            CurrentBackupId = current.Id,
            HasDrift = hasDrift,
            AddedLinesCount = added.Count,
            RemovedLinesCount = removed.Count,
            ModifiedLinesCount = Math.Max(added.Count, removed.Count),
            DifferencesJson = JsonSerializer.Serialize(diffSummary),
            DetectedAtUtc = DateTime.UtcNow,
            Severity = hasDrift ? ThreatSeverity.Medium : ThreatSeverity.Low
        };

        await _unitOfWork.ConfigurationDrifts.AddAsync(driftRecord, cancellationToken);

        if (hasDrift)
        {
            var existingIndicator = await _unitOfWork.ThreatIndicators.GetActiveIndicatorAsync(
                _tenantContext.TenantId,
                ThreatType.ConfigurationDrift,
                sourceIp: null,
                targetDeviceId: deviceId,
                targetUser: null,
                cancellationToken: cancellationToken);

            if (existingIndicator != null)
            {
                existingIndicator.AttemptCount += 1;
                existingIndicator.LastDetectedAtUtc = DateTime.UtcNow;
                existingIndicator.IndicatorMetadataJson = driftRecord.DifferencesJson;
                _unitOfWork.ThreatIndicators.Update(existingIndicator);
            }
            else
            {
                var indicator = new ThreatIndicator
                {
                    TenantId = _tenantContext.TenantId,
                    ThreatType = ThreatType.ConfigurationDrift,
                    Severity = ThreatSeverity.Medium,
                    Status = ThreatStatus.Active,
                    Title = $"Configuration Drift on Device {deviceId}",
                    Description = $"Device configuration diverged from baseline with {added.Count} added and {removed.Count} removed line(s).",
                    TargetDeviceId = deviceId,
                    AttemptCount = 1,
                    FirstDetectedAtUtc = DateTime.UtcNow,
                    LastDetectedAtUtc = DateTime.UtcNow,
                    IndicatorMetadataJson = driftRecord.DifferencesJson
                };
                await _unitOfWork.ThreatIndicators.AddAsync(indicator, cancellationToken);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return driftRecord;
    }

    public async Task<IReadOnlyList<ConfigurationDriftRecord>> AnalyzeAllDevicesDriftAsync(CancellationToken cancellationToken = default)
    {
        var (devices, _) = await _unitOfWork.Devices.GetPagedAsync(
            pageIndex: 1,
            pageSize: 1000,
            searchTerm: null,
            deviceType: null,
            status: null,
            cancellationToken: cancellationToken);

        var results = new List<ConfigurationDriftRecord>();

        foreach (var device in devices)
        {
            var record = await AnalyzeDeviceDriftAsync(device.Id, cancellationToken);
            results.Add(record);
        }

        return results;
    }
}