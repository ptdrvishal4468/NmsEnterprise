using System.Net;
using Nms.Application.Notifications.Commands.CreateNotificationTemplate;
using Nms.Domain.Enums;
using Xunit;

namespace Nms.IntegrationTests.Notifications;

public class NotificationTemplatesEndpointTests
{
    [Fact]
    public void NotificationTemplate_Command_Validation_ShouldFail_WhenNameIsEmpty()
    {
        // Arrange
        var command = new CreateNotificationTemplateCommand(
            Name: "",
            Channel: NotificationChannel.Email,
            SubjectTemplate: "Subject",
            BodyTemplate: "Body",
            RecipientTarget: "admin@domain.com");

        var validator = new CreateNotificationTemplateCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateNotificationTemplateCommand.Name));
    }

    [Fact]
    public void NotificationTemplate_Command_Validation_ShouldSucceed_WhenValid()
    {
        // Arrange
        var command = new CreateNotificationTemplateCommand(
            Name: "Critical CPU Webhook",
            Channel: NotificationChannel.Webhook,
            SubjectTemplate: "Alert {AlertSeverity}",
            BodyTemplate: "Metric breached: {MetricValue}",
            RecipientTarget: "https://api.enterprise.com/webhooks");

        var validator = new CreateNotificationTemplateCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
    }
}