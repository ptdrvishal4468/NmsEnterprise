namespace Nms.Application.Dashboard.Dtos;

public sealed record TopAlertRuleDto(
    Guid AlertRuleId,
    string RuleName,
    int ActiveAlertCount);

public sealed record AlertSummaryDto
{
    public int TotalActiveAlerts { get; init; }
    public int TotalAcknowledgedAlerts { get; init; }
    public int TotalSuppressedAlerts { get; init; }
    public IReadOnlyDictionary<string, int> SeverityDistribution { get; init; } = new Dictionary<string, int>();
    public IReadOnlyDictionary<string, int> MetricTypeDistribution { get; init; } = new Dictionary<string, int>();
    public IReadOnlyList<TopAlertRuleDto> TopFiringRules { get; init; } = Array.Empty<TopAlertRuleDto>();
    public DateTime GeneratedAtUtc { get; init; } = DateTime.UtcNow;
}