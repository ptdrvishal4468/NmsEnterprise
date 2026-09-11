using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Nms.Application.Common.Interfaces;
using Nms.Infrastructure.Ticketing.Options;

namespace Nms.Infrastructure.Ticketing.Jira;

public class JiraClient : IJiraClient
{
    private readonly HttpClient _httpClient;
    private readonly JiraOptions _options;

    public JiraClient(HttpClient httpClient, IOptions<JiraOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<(string IssueId, string IssueKey, string? Url)> CreateIssueAsync(
        string summary,
        string description,
        string issueType,
        string priority,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.BaseUrl))
        {
            var fakeId = Random.Shared.Next(10000, 99999).ToString();
            var fakeKey = $"{_options.ProjectKey}-{fakeId}";
            return (fakeId, fakeKey, $"https://jira.local/browse/{fakeKey}");
        }

        var requestUri = $"{_options.BaseUrl.TrimEnd('/')}/rest/api/2/issue";
        var payload = JsonSerializer.Serialize(new
        {
            fields = new
            {
                project = new { key = _options.ProjectKey },
                summary = summary,
                description = description,
                issuetype = new { name = issueType }
            }
        });

        using var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
        var authHeader = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_options.Username}:{_options.ApiToken}"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authHeader);
        request.Content = new StringContent(payload, Encoding.UTF8, "application/json");

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        using var doc = JsonDocument.Parse(json);
        var id = doc.RootElement.GetProperty("id").GetString() ?? Guid.NewGuid().ToString();
        var key = doc.RootElement.GetProperty("key").GetString() ?? $"{_options.ProjectKey}-1";
        var url = $"{_options.BaseUrl.TrimEnd('/')}/browse/{key}";

        return (id, key, url);
    }

    public async Task UpdateIssueAsync(string issueIdOrKey, string statusTransitionId, string? comment, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.BaseUrl)) return;

        var requestUri = $"{_options.BaseUrl.TrimEnd('/')}/rest/api/2/issue/{issueIdOrKey}/transitions";
        var payload = JsonSerializer.Serialize(new
        {
            transition = new { id = statusTransitionId }
        });

        using var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
        var authHeader = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_options.Username}:{_options.ApiToken}"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authHeader);
        request.Content = new StringContent(payload, Encoding.UTF8, "application/json");

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task<string?> GetIssueStatusAsync(string issueIdOrKey, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.BaseUrl)) return "Open";

        var requestUri = $"{_options.BaseUrl.TrimEnd('/')}/rest/api/2/issue/{issueIdOrKey}?fields=status";
        using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        var authHeader = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_options.Username}:{_options.ApiToken}"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authHeader);

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode) return null;

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.GetProperty("fields").GetProperty("status").GetProperty("name").GetString();
    }
}