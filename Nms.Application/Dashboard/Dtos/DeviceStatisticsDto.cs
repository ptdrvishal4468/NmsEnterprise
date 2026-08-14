namespace Nms.Application.Dashboard.Dtos;

public sealed record DeviceStatisticsDto
{
    public int TotalDevices { get; init; }
    public IReadOnlyDictionary<string, int> StatusDistribution { get; init; } = new Dictionary<string, int>();
    public IReadOnlyDictionary<string, int> TypeDistribution { get; init; } = new Dictionary<string, int>();
    public IReadOnlyDictionary<string, int> VendorDistribution { get; init; } = new Dictionary<string, int>();
    public IReadOnlyDictionary<string, int> SiteDistribution { get; init; } = new Dictionary<string, int>();
    public DateTime GeneratedAtUtc { get; init; } = DateTime.UtcNow;
}