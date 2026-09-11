using Microsoft.Extensions.Logging;
using Moq;
using Nms.Application.Common.Interfaces;
using Nms.Application.Notifications.Services;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;
using Xunit;

namespace Nms.UnitTests.Notifications;

public class NotificationDispatcherTests
{
    private readonly Mock<INotificationTemplateRepository> _templateRepoMock = new();
    private readonly Mock<INotificationLogRepository> _logRepoMock = new();
    private readonly Mock<IDeviceRepository> _deviceRepoMock = new();
    private readonly Mock<INotificationTemplateEngine> _engineMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ILogger<NotificationDispatcher>> _loggerMock = new();
    private readonly Mock<INotificationProvider> _providerMock = new();

    [Fact]
    public async Task DispatchAlertNotificationAsync_ShouldSendNotificationAndLog_WhenTemplateExists()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var alert = new Alert(tenantId, Guid.NewGuid(), Guid.NewGuid(), MetricType.CpuUsage, AlertSeverity.Critical, 90m, 80m, "CPU Critical");

        var template = new NotificationTemplate(tenantId, "Email Rule", NotificationChannel.Email, "Subject {MetricType}", "Body {MetricValue}", "admin@domain.com");

        _templateRepoMock
            .Setup(r => r.GetActiveTemplatesForTenantAsync(tenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NotificationTemplate> { template });

        _providerMock.Setup(p => p.Channel).Returns(NotificationChannel.Email);
        _providerMock
            .Setup(p => p.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _engineMock
            .Setup(e => e.Render(It.IsAny<string>(), It.IsAny<Alert>(), It.IsAny<Device?>()))
            .Returns("Rendered Content");

        var dispatcher = new NotificationDispatcher(
            new[] { _providerMock.Object },
            _templateRepoMock.Object,
            _logRepoMock.Object,
            _deviceRepoMock.Object,
            _engineMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);

        // Act
        await dispatcher.DispatchAlertNotificationAsync(alert);

        // Assert
        _logRepoMock.Verify(l => l.AddAsync(It.IsAny<NotificationLog>(), It.IsAny<CancellationToken>()), Times.Once);
        _providerMock.Verify(p => p.SendAsync("admin@domain.com", "Rendered Content", "Rendered Content", It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}