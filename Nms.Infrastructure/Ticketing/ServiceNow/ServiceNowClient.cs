using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Nms.Application.Common.Interfaces;
using Nms.Infrastructure.Ticketing.Options;

namespace Nms.Infrastructure.Ticketing.ServiceNow;

public class ServiceNowClient : IServiceNowClient
{
    private readonly HttpClient _httpClient;
    private readonly ServiceNowOptions _options;

    public ServiceNowClient(HttpClient httpClient, IOptions<ServiceNowOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<(string SysId, string Number, string? Url)> CreateIncidentAsync(
        string shortDescription,
        string description,
        string urgency,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.InstanceUrl))
        {
            var fakeSysId = Guid.NewGuid().ToString("N");
            return (fakeSysId, $"INC{Random.Shared.Next(100000, 999999)}", $"https://servicenow.local/incident/{fakeSysId}");
        }

        var requestUri = $"{_options.InstanceUrl.TrimEnd('/')}/api/now/table/incident";
        var payload = JsonSerializer.Serialize(new
        {
            short_description = shortDescription,
            description = description,
            urgency = urgency
        });

        using var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
        var authHeader = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_options.Username}:{_options.Password}"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authHeader);
        request.Content = new StringContent(payload, Encoding.UTF8, "application/json");

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        using var doc = JsonDocument.Parse(json);
        var result = doc.RootElement.GetProperty("result");
        var sysId = result.GetProperty("sys_id").GetString() ?? Guid.NewGuid().ToString();
        var number = result.GetProperty("number").GetString() ?? "INC000000";
        var url = $"{_options.InstanceUrl.TrimEnd('/')}/nav_to.do?uri=incident.do?sys_id={sysId}";

        return (sysId, number, url);
    }

    public async Task UpdateIncidentAsync(string sysId, string status, string? comments, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.InstanceUrl)) return;

        var requestUri = $"{_options.InstanceUrl.TrimEnd('/')}/api/now/table/incident/{sysId}";
        var payload = JsonSerializer.Serialize(new { state = status, comments = comments });

        using var request = new HttpRequestMessage(HttpMethod.Patch, requestUri);
        var authHeader = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_options.Username}:{_options.Password}"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authHeader);
        request.Content = new StringContent(payload, Encoding.UTF8, "application/json");

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task<string?> GetIncidentStatusAsync(string sysId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.InstanceUrl)) return "Open";

        var requestUri = $"{_options.InstanceUrl.TrimEnd('/')}/api/now/table/incident/{sysId}";
        using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        var authHeader = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_options.Username}:{_options.Password}"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authHeader);

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode) return null;

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.GetProperty("result").GetProperty("state").GetString();
    }
}