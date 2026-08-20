namespace Nms.Application.Dashboard.Dtos;

public sealed class PerformanceDashboardDto
{
    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
    public long ProcessAllocatedMemoryBytes { get; set; }
    public long WorkingSetBytes { get; set; }
    public int Gen0Collections { get; set; }
    public int Gen1Collections { get; set; }
    public int Gen2Collections { get; set; }
    public int ThreadCount { get; set; }
    public int QueueCapacity { get; set; }
    public int QueuedJobsCount { get; set; }
    public double QueueUtilizationPercent { get; set; }
    public string CacheStatus { get; set; } = "Unknown";
    public string DatabaseStatus { get; set; } = "Unknown";
}