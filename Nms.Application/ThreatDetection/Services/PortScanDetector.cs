using System.Text.Json;
using Nms.Application.Common.Interfaces;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;

namespace Nms.Application.ThreatDetection.Services;

public class PortScanDetector : IPortScanDetector
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;

    public PortScanDetector(IUnitOfWork unitOfWork, ITenantContext tenantContext)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<IReadOnlyList<ThreatIndicator>> DetectPortScanIndicatorsAsync(
        int timeWindowMinutes = 60,
        CancellationToken cancellationToken = default)
    {
        var rule = await _unitOfWork.ThreatDetectionRules.GetRuleByThreatTypeAsync(_tenantContext.TenantId, ThreatType.PortScanIndicator, cancellationToken);
        var window = rule?.TimeWindowMinutes ?? timeWindowMinutes;
        var threshold = rule?.FailureThreshold ?? 10;
        var severity = rule?.DefaultSeverity ?? ThreatSeverity.High;

        var fromUtc = DateTime.UtcNow.AddMinutes(-window);
        var (syslogs, _) = await _unitOfWork.Syslogs.GetSyslogsPagedAsync(
            tenantId: _tenantContext.TenantId,
            deviceId: null,
            severity: null,
            facility: null,
            sourceIp: null,
            searchKeyword: null,
            startDateUtc: fromUtc,
            endDateUtc: null,
            pageNumber: 1,
            pageSize: 1000,
            cancellationToken: cancellationToken);

        var scanSyslogs = syslogs
            .Where(s => s.Message.Contains("scan", StringComparison.OrdinalIgnoreCase) ||
                        s.Message.Contains("denied", StringComparison.OrdinalIgnoreCase) ||
                        s.Message.Contains("closed port", StringComparison.OrdinalIgnoreCase))
            .GroupBy(s => string.IsNullOrWhiteSpace(s.Hostname) ? s.SourceIpAddress : s.Hostname)
            .Where(g => g.Count() >= threshold)
            .ToList();

        var detected = new List<ThreatIndicator>();

        foreach (var group in scanSyslogs)
        {
            var host = group.Key;
            var count = group.Count();

            var existing = await _unitOfWork.ThreatIndicators.GetActiveIndicatorAsync(
                _tenantContext.TenantId,
                ThreatType.PortScanIndicator,
                sourceIp: host,
                targetDeviceId: null,
                targetUser: null,
                cancellationToken: cancellationToken);

            if (existing != null)
            {
                existing.AttemptCount = count;
                existing.LastDetectedAtUtc = DateTime.UtcNow;
                existing.Severity = severity;
                _unitOfWork.ThreatIndicators.Update(existing);
                detected.Add(existing);
            }
            else
            {
                var indicator = new ThreatIndicator
                {
                    TenantId = _tenantContext.TenantId,
                    ThreatType = ThreatType.PortScanIndicator,
                    Severity = severity,
                    Status = ThreatStatus.Active,
                    Title = $"Port Scan Signal Detected from Host {host}",
                    Description = $"Observed {count} port-scan or connection-probe triggers within {window} minutes.",
                    SourceIp = host,
                    AttemptCount = count,
                    FirstDetectedAtUtc = group.Min(g => g.TimestampUtc),
                    LastDetectedAtUtc = group.Max(g => g.TimestampUtc),
                    IndicatorMetadataJson = JsonSerializer.Serialize(new { Triggers = count, SampleMessages = group.Take(3).Select(m => m.Message) })
                };

                await _unitOfWork.ThreatIndicators.AddAsync(indicator, cancellationToken);
                detected.Add(indicator);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return detected;
    }
}