using Moq;
using Nms.Application.Common.Interfaces;
using Nms.Application.Events.Commands.RecordEvent;
using Nms.Domain.Enums;
using Xunit;

namespace Nms.UnitTests.Events;

public class RecordEventCommandHandlerTests
{
    private readonly Mock<IEventPublisher> _eventPublisherMock;
    private readonly RecordEventCommandHandler _handler;

    public RecordEventCommandHandlerTests()
    {
        _eventPublisherMock = new Mock<IEventPublisher>();
        _handler = new RecordEventCommandHandler(_eventPublisherMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldPublishAndReturnEventId()
    {
        // Arrange
        var expectedId = Guid.NewGuid();
        var command = new RecordEventCommand(
            Category: EventCategory.Telemetry,
            Severity: EventSeverity.Error,
            Source: "TelemetryEngine",
            Message: "High CPU usage detected",
            DeviceId: Guid.NewGuid());

        _eventPublisherMock.Setup(x => x.PublishAsync(
                command.Category,
                command.Severity,
                command.Source,
                command.Message,
                command.DeviceId,
                command.CorrelationId,
                command.ParentEventId,
                command.MetadataJson,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(expectedId, result);
        _eventPublisherMock.Verify(x => x.PublishAsync(
            command.Category,
            command.Severity,
            command.Source,
            command.Message,
            command.DeviceId,
            command.CorrelationId,
            command.ParentEventId,
            command.MetadataJson,
            It.IsAny<CancellationToken>()), Times.Once);
    }
}