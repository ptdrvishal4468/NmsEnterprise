using Nms.Domain.Enums;

namespace Nms.Infrastructure.Ticketing.Options;

public class TicketingOptions
{
    public const string SectionName = "Ticketing";

    public TicketingProviderType DefaultProvider { get; set; } = TicketingProviderType.ServiceNow;
    public bool AutoSyncEnabled { get; set; } = true;
    public int SyncIntervalMinutes { get; set; } = 15;
    public int MaxRetryAttempts { get; set; } = 3;
    public int TimeoutSeconds { get; set; } = 30;
    public ServiceNowOptions ServiceNow { get; set; } = new();
    public JiraOptions Jira { get; set; } = new();
}