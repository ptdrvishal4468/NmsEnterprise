using System.Text.Json;
using Nms.Application.Common.Interfaces;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;

namespace Nms.Application.ThreatDetection.Services;

public class UnauthorizedAccessDetector : IUnauthorizedAccessDetector
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;

    public UnauthorizedAccessDetector(IUnitOfWork unitOfWork, ITenantContext tenantContext)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<IReadOnlyList<ThreatIndicator>> DetectUnauthorizedAccessPatternsAsync(
        int timeWindowMinutes = 60,
        CancellationToken cancellationToken = default)
    {
        var rule = await _unitOfWork.ThreatDetectionRules.GetRuleByThreatTypeAsync(_tenantContext.TenantId, ThreatType.UnauthorizedAccess, cancellationToken);
        var window = rule?.TimeWindowMinutes ?? timeWindowMinutes;
        var threshold = rule?.FailureThreshold ?? 3;
        var severity = rule?.DefaultSeverity ?? ThreatSeverity.Critical;

        var fromUtc = DateTime.UtcNow.AddMinutes(-window);
        var (auditLogs, _) = await _unitOfWork.AuditLogs.SearchAuditLogsAsync(
            tenantId: _tenantContext.TenantId,
            fromUtc: fromUtc,
            toUtc: null,
            userId: null,
            action: null,
            category: AuditCategory.Security,
            status: AuditStatus.Failure,
            entityName: null,
            entityId: null,
            searchTerm: null,
            pageIndex: 1,
            pageSize: 1000,
            cancellationToken: cancellationToken);

        var groupedByUser = auditLogs
            .Where(a => !string.IsNullOrWhiteSpace(a.Username))
            .GroupBy(a => a.Username!)
            .Where(g => g.Count() >= threshold)
            .ToList();

        var detected = new List<ThreatIndicator>();

        foreach (var group in groupedByUser)
        {
            var user = group.Key;
            var count = group.Count();
            var sourceIp = group.FirstOrDefault(g => !string.IsNullOrEmpty(g.IpAddress))?.IpAddress ?? "Unknown";

            var existing = await _unitOfWork.ThreatIndicators.GetActiveIndicatorAsync(
                _tenantContext.TenantId,
                ThreatType.UnauthorizedAccess,
                sourceIp: null,
                targetDeviceId: null,
                targetUser: user,
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
                var distinctActions = group.Select(g => g.Action).Where(a => !string.IsNullOrEmpty(a)).Distinct().ToList();
                var indicator = new ThreatIndicator
                {
                    TenantId = _tenantContext.TenantId,
                    ThreatType = ThreatType.UnauthorizedAccess,
                    Severity = severity,
                    Status = ThreatStatus.Active,
                    Title = $"Unauthorized Resource Access Pattern by {user}",
                    Description = $"User triggered {count} unauthorized/denied access violations within {window} minutes.",
                    SourceIp = sourceIp,
                    TargetUser = user,
                    AttemptCount = count,
                    FirstDetectedAtUtc = group.Min(g => g.TimestampUtc),
                    LastDetectedAtUtc = group.Max(g => g.TimestampUtc),
                    IndicatorMetadataJson = JsonSerializer.Serialize(new { DeniedActions = distinctActions })
                };

                await _unitOfWork.ThreatIndicators.AddAsync(indicator, cancellationToken);
                detected.Add(indicator);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return detected;
    }
}