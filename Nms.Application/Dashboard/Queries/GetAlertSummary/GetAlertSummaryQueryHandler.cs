using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Dashboard.Dtos;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;

namespace Nms.Application.Dashboard.Queries.GetAlertSummary;

public class GetAlertSummaryQueryHandler : IRequestHandler<GetAlertSummaryQuery, AlertSummaryDto>
{
    private readonly IAlertRepository _alertRepository;
    private readonly IAlertRuleRepository _alertRuleRepository;
    private readonly ICacheService? _cacheService;

    public GetAlertSummaryQueryHandler(
        IAlertRepository alertRepository,
        IAlertRuleRepository alertRuleRepository,
        ICacheService? cacheService = null)
    {
        _alertRepository = alertRepository;
        _alertRuleRepository = alertRuleRepository;
        _cacheService = cacheService;
    }

    public async Task<AlertSummaryDto> Handle(GetAlertSummaryQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"dashboard:alert-summary:{request.TopRulesLimit}";
        if (_cacheService != null)
        {
            var cached = await _cacheService.GetAsync<AlertSummaryDto>(cacheKey, cancellationToken);
            if (cached != null)
            {
                return cached;
            }
        }

        // 1. Fetch non-resolved alerts (Active, Acknowledged, Suppressed)
        var unresolvedAlerts = await _alertRepository.FindAsync(
            a => a.State != AlertState.Resolved,
            cancellationToken);

        int totalActive = unresolvedAlerts.Count(a => a.State == AlertState.Active);
        int totalAcknowledged = unresolvedAlerts.Count(a => a.State == AlertState.Acknowledged);
        int totalSuppressed = unresolvedAlerts.Count(a => a.State == AlertState.Suppressed);

        // 2. Active alerts breakdown
        var activeAlerts = unresolvedAlerts.Where(a => a.State == AlertState.Active).ToList();

        var severityDistribution = activeAlerts
            .GroupBy(a => a.Severity.ToString())
            .ToDictionary(g => g.Key, g => g.Count());

        var metricTypeDistribution = activeAlerts
            .GroupBy(a => a.MetricType.ToString())
            .ToDictionary(g => g.Key, g => g.Count());

        // 3. Resolve top firing alert rules
        var rules = await _alertRuleRepository.GetAllAsync(cancellationToken);
        var ruleNameLookup = rules.ToDictionary(r => r.Id, r => r.Name);

        var topFiringRules = activeAlerts
            .GroupBy(a => a.AlertRuleId)
            .OrderByDescending(g => g.Count())
            .Take(Math.Max(1, request.TopRulesLimit))
            .Select(g => new TopAlertRuleDto(
                g.Key,
                ruleNameLookup.TryGetValue(g.Key, out var name) ? name : "Unknown Rule",
                g.Count()))
            .ToList();

        var result = new AlertSummaryDto
        {
            TotalActiveAlerts = totalActive,
            TotalAcknowledgedAlerts = totalAcknowledged,
            TotalSuppressedAlerts = totalSuppressed,
            SeverityDistribution = severityDistribution,
            MetricTypeDistribution = metricTypeDistribution,
            TopFiringRules = topFiringRules,
            GeneratedAtUtc = DateTime.UtcNow
        };

        if (_cacheService != null)
        {
            await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromSeconds(30), cancellationToken);
        }

        return result;
    }
}