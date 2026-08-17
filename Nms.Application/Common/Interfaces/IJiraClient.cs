namespace Nms.Application.Common.Interfaces;

public interface IJiraClient
{
    Task<(string IssueId, string IssueKey, string? Url)> CreateIssueAsync(string summary, string description, string issueType, string priority, CancellationToken cancellationToken = default);
    Task UpdateIssueAsync(string issueIdOrKey, string statusTransitionId, string? comment, CancellationToken cancellationToken = default);
    Task<string?> GetIssueStatusAsync(string issueIdOrKey, CancellationToken cancellationToken = default);
}