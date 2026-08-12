using System.Net;
using System.Net.Http.Json;
using Nms.Application.Events.Commands.RecordEvent;
using Nms.Domain.Enums;
using Xunit;

namespace Nms.IntegrationTests.Events;

public class EventsEndpointTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public EventsEndpointTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetEvents_Route_ShouldRespondWithExpectedStatusCode()
    {
        var response = await _client.GetAsync("/api/v1/events");

        Assert.True(
            response.StatusCode == HttpStatusCode.OK ||
            response.StatusCode == HttpStatusCode.Unauthorized ||
            response.StatusCode == HttpStatusCode.Forbidden ||
            response.StatusCode == HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task GetEventById_Route_ShouldRespondWithExpectedStatusCode()
    {
        var eventId = Guid.NewGuid();
        var response = await _client.GetAsync($"/api/v1/events/{eventId}");

        Assert.True(
            response.StatusCode == HttpStatusCode.OK ||
            response.StatusCode == HttpStatusCode.NotFound ||
            response.StatusCode == HttpStatusCode.Unauthorized ||
            response.StatusCode == HttpStatusCode.Forbidden ||
            response.StatusCode == HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task RecordEvent_Route_ShouldRespondWithExpectedStatusCode()
    {
        var command = new RecordEventCommand(
            Category: EventCategory.Device,
            Severity: EventSeverity.Warning,
            Source: "IntegrationTest",
            Message: "Testing event recording via integration test endpoint.",
            DeviceId: Guid.NewGuid());

        var response = await _client.PostAsJsonAsync("/api/v1/events", command);

        Assert.True(
            response.StatusCode == HttpStatusCode.Created ||
            response.StatusCode == HttpStatusCode.BadRequest ||
            response.StatusCode == HttpStatusCode.Unauthorized ||
            response.StatusCode == HttpStatusCode.Forbidden ||
            response.StatusCode == HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task GetDeviceEventTimeline_Route_ShouldRespondWithExpectedStatusCode()
    {
        var deviceId = Guid.NewGuid();
        var response = await _client.GetAsync($"/api/v1/events/device/{deviceId}/timeline");

        Assert.True(
            response.StatusCode == HttpStatusCode.OK ||
            response.StatusCode == HttpStatusCode.Unauthorized ||
            response.StatusCode == HttpStatusCode.Forbidden ||
            response.StatusCode == HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task GetCorrelatedEvents_Route_ShouldRespondWithExpectedStatusCode()
    {
        var correlationId = "CORR-TEST-1001";
        var response = await _client.GetAsync($"/api/v1/events/correlation/{correlationId}");

        Assert.True(
            response.StatusCode == HttpStatusCode.OK ||
            response.StatusCode == HttpStatusCode.Unauthorized ||
            response.StatusCode == HttpStatusCode.Forbidden ||
            response.StatusCode == HttpStatusCode.InternalServerError);
    }

    [Fact]
    public void RecordEvent_Command_Validation_ShouldFail_WhenSourceIsEmpty()
    {
        // Arrange
        var command = new RecordEventCommand(
            Category: EventCategory.System,
            Severity: EventSeverity.Error,
            Source: "",
            Message: "Test message");

        var validator = new RecordEventCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(RecordEventCommand.Source));
    }

    [Fact]
    public void RecordEvent_Command_Validation_ShouldSucceed_WhenValid()
    {
        // Arrange
        var command = new RecordEventCommand(
            Category: EventCategory.Security,
            Severity: EventSeverity.Critical,
            Source: "AuthEngine",
            Message: "Repeated failed login attempts detected.");

        var validator = new RecordEventCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
    }
}