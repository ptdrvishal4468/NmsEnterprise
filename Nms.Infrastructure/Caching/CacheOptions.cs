namespace Nms.Infrastructure.Caching;

public class CacheOptions
{
    public const string SectionName = "CacheOptions";

    public int DefaultExpirationMinutes { get; set; } = 5;
    public int DashboardExpirationSeconds { get; set; } = 30;
    public int RuleCacheExpirationMinutes { get; set; } = 15;
    public bool EnableRedis { get; set; } = true;
}