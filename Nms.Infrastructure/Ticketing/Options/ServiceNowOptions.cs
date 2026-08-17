namespace Nms.Infrastructure.Ticketing.Options;

public class ServiceNowOptions
{
    public const string SectionName = "Ticketing:ServiceNow";
    public string InstanceUrl { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}