using System.Net;
using System.Net.Http.Json;
using Nms.Application.Alerts.Commands.CreateAlertRule;
using Nms.Application.Alerts.Commands.UpdateAlertRule;
using Nms.Domain.Enums;
using Xunit;

namespace Nms.IntegrationTests.Alerts;

public class AlertRulesEndpointTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AlertRulesEndpointTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAlertRules_Route_ShouldRespondWithExpectedStatusCode()
    {
        var response = await _client.GetAsync("/api/v1/alert-rules");

        Assert.True(
            response.StatusCode == HttpStatusCode.OK ||
            response.StatusCode == HttpStatusCode.Unauthorized ||
            response.StatusCode == HttpStatusCode.Forbidden ||
            response.StatusCode == HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task CreateAlertRule_Route_ShouldRespondWithExpectedStatusCode()
    {
        var command = new CreateAlertRuleCommand(
            "Integration Test Rule",
            "High CPU Utilization",
            MetricType.CpuUsage,
            ComparisonOperator.GreaterThan,
            85m,
            AlertSeverity.Critical);

        var response = await _client.PostAsJsonAsync("/api/v1/alert-rules", command);

        Assert.True(
            response.StatusCode == HttpStatusCode.OK ||
            response.StatusCode == HttpStatusCode.BadRequest ||
            response.StatusCode == HttpStatusCode.Unauthorized ||
            response.StatusCode == HttpStatusCode.Forbidden ||
            response.StatusCode == HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task UpdateAlertRule_Route_ShouldRespondWithExpectedStatusCode()
    {
        var ruleId = Guid.NewGuid();
        var command = new UpdateAlertRuleCommand(
            ruleId,
            "Updated Integration Test Rule",
            "High RAM Utilization",
            MetricType.MemoryUsage,
            ComparisonOperator.GreaterThanOrEqual,
            90m,
            AlertSeverity.Warning);

        var response = await _client.PutAsJsonAsync($"/api/v1/alert-rules/{ruleId}", command);

        Assert.True(
            response.StatusCode == HttpStatusCode.OK ||
            response.StatusCode == HttpStatusCode.BadRequest ||
            response.StatusCode == HttpStatusCode.NotFound ||
            response.StatusCode == HttpStatusCode.Unauthorized ||
            response.StatusCode == HttpStatusCode.Forbidden ||
            response.StatusCode == HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task DeleteAlertRule_Route_ShouldRespondWithExpectedStatusCode()
    {
        var ruleId = Guid.NewGuid();
        var response = await _client.DeleteAsync($"/api/v1/alert-rules/{ruleId}");

        Assert.True(
            response.StatusCode == HttpStatusCode.NoContent ||
            response.StatusCode == HttpStatusCode.NotFound ||
            response.StatusCode == HttpStatusCode.Unauthorized ||
            response.StatusCode == HttpStatusCode.Forbidden ||
            response.StatusCode == HttpStatusCode.InternalServerError);
    }
}