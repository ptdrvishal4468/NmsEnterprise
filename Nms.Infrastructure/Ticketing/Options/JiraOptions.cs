namespace Nms.Infrastructure.Ticketing.Options;

public class JiraOptions
{
    public const string SectionName = "Ticketing:Jira";
    public string BaseUrl { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string ApiToken { get; set; } = string.Empty;
    public string ProjectKey { get; set; } = "NMS";
    public string DefaultIssueType { get; set; } = "Task";
}