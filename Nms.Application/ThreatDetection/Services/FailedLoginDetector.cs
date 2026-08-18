using System.Text.Json;
using Nms.Application.Common.Interfaces;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;

namespace Nms.Application.ThreatDetection.Services;

public class FailedLoginDetector : IFailedLoginDetector
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;

    public FailedLoginDetector(IUnitOfWork unitOfWork, ITenantContext tenantContext)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<IReadOnlyList<ThreatIndicator>> DetectFailedLoginsAsync(
        int? thresholdOverride = null,
        int? timeWindowMinutesOverride = null,
        CancellationToken cancellationToken = default)
    {
        var rule = await _unitOfWork.ThreatDetectionRules.GetRuleByThreatTypeAsync(_tenantContext.TenantId, ThreatType.FailedLogin, cancellationToken);
        var threshold = thresholdOverride ?? rule?.FailureThreshold ?? 5;
        var windowMinutes = timeWindowMinutesOverride ?? rule?.TimeWindowMinutes ?? 15;
        var severity = rule?.DefaultSeverity ?? ThreatSeverity.High;

        var fromUtc = DateTime.UtcNow.AddMinutes(-windowMinutes);
        var (auditLogs, _) = await _unitOfWork.AuditLogs.SearchAuditLogsAsync(
            tenantId: _tenantContext.TenantId,
            fromUtc: fromUtc,
            toUtc: null,
            userId: null,
            action: null,
            category: AuditCategory.Authentication,
            status: AuditStatus.Failure,
            entityName: null,
            entityId: null,
            searchTerm: null,
            pageIndex: 1,
            pageSize: 1000,
            cancellationToken: cancellationToken);

        var groupedByIp = auditLogs
            .Where(a => !string.IsNullOrWhiteSpace(a.IpAddress))
            .GroupBy(a => a.IpAddress)
            .Where(g => g.Count() >= threshold)
            .ToList();

        var detectedIndicators = new List<ThreatIndicator>();

        foreach (var group in groupedByIp)
        {
            var ip = group.Key!;
            var attempts = group.Count();
            var distinctUsers = group.Select(g => g.Username).Where(u => !string.IsNullOrEmpty(u)).Distinct().ToList();
            var targetUser = distinctUsers.Count == 1 ? distinctUsers[0]! : $"Multiple ({distinctUsers.Count} users)";

            var existing = await _unitOfWork.ThreatIndicators.GetActiveIndicatorAsync(
                _tenantContext.TenantId,
                ThreatType.FailedLogin,
                sourceIp: ip,
                targetDeviceId: null,
                targetUser: targetUser,
                cancellationToken: cancellationToken);

            if (existing != null)
            {
                existing.AttemptCount = attempts;
                existing.LastDetectedAtUtc = DateTime.UtcNow;
                existing.Severity = severity;
                existing.IndicatorMetadataJson = JsonSerializer.Serialize(new { TargetedUsers = distinctUsers, Attempts = attempts });
                _unitOfWork.ThreatIndicators.Update(existing);
                detectedIndicators.Add(existing);
            }
            else
            {
                var indicator = new ThreatIndicator
                {
                    TenantId = _tenantContext.TenantId,
                    ThreatType = ThreatType.FailedLogin,
                    Severity = severity,
                    Status = ThreatStatus.Active,
                    Title = $"Repeated Failed Login Attempts from {ip}",
                    Description = $"Detected {attempts} failed authentication attempts within {windowMinutes} minutes.",
                    SourceIp = ip,
                    TargetUser = targetUser,
                    AttemptCount = attempts,
                    FirstDetectedAtUtc = group.Min(g => g.TimestampUtc),
                    LastDetectedAtUtc = group.Max(g => g.TimestampUtc),
                    IndicatorMetadataJson = JsonSerializer.Serialize(new { TargetedUsers = distinctUsers, Attempts = attempts })
                };

                await _unitOfWork.ThreatIndicators.AddAsync(indicator, cancellationToken);
                detectedIndicators.Add(indicator);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return detectedIndicators;
    }
}