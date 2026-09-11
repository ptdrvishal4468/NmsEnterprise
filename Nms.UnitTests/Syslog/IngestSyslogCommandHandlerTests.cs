using Moq;
using Nms.Application.Common.Interfaces;
using Nms.Application.Syslog.Commands.IngestSyslogMessage;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;
using Xunit;

namespace Nms.UnitTests.Syslog;

public class IngestSyslogMessageCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<ISyslogRepository> _syslogRepoMock;
    private readonly Mock<IDeviceRepository> _deviceRepoMock;
    private readonly Mock<IEventPublisher> _eventPublisherMock;
    private readonly IngestSyslogMessageCommandHandler _handler;

    public IngestSyslogMessageCommandHandlerTests()
    {
        _uowMock = new Mock<IUnitOfWork>();
        _syslogRepoMock = new Mock<ISyslogRepository>();
        _deviceRepoMock = new Mock<IDeviceRepository>();
        _eventPublisherMock = new Mock<IEventPublisher>();

        _uowMock.Setup(u => u.Syslogs).Returns(_syslogRepoMock.Object);
        _uowMock.Setup(u => u.Devices).Returns(_deviceRepoMock.Object);

        _handler = new IngestSyslogMessageCommandHandler(_uowMock.Object, _eventPublisherMock.Object);
    }

    [Fact]
    public async Task Handle_ValidSyslogMessage_AddsToRepositoryAndSaves()
    {
        // Arrange
        var command = new IngestSyslogMessageCommand(
            TenantId: Guid.NewGuid(),
            SourceIpAddress: "192.168.1.1",
            Facility: SyslogFacility.Daemon,
            Severity: SyslogSeverity.Informational,
            TimestampUtc: DateTime.UtcNow,
            Hostname: "host1",
            AppTag: "service",
            ProcessId: null,
            MessageId: null,
            Message: "Service started successfully",
            RawMessage: "<30>Oct 11 22:14:15 host1 service: Service started successfully",
            IsMalformed: false);

        _deviceRepoMock.Setup(d => d.GetByIpAddressAsync("192.168.1.1", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Device?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        _syslogRepoMock.Verify(r => r.AddAsync(It.Is<SyslogMessage>(m => m.Message == "Service started successfully"), It.IsAny<CancellationToken>()), Times.Once);
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_HighSeveritySyslog_PublishesOperationalEvent()
    {
        // Arrange
        var command = new IngestSyslogMessageCommand(
            TenantId: Guid.NewGuid(),
            SourceIpAddress: "10.0.0.5",
            Facility: SyslogFacility.Kernel,
            Severity: SyslogSeverity.Critical,
            TimestampUtc: DateTime.UtcNow,
            Hostname: "core-switch",
            AppTag: "kernel",
            ProcessId: null,
            MessageId: null,
            Message: "Hardware fault detected on power supply",
            RawMessage: "<2>Hardware fault detected on power supply",
            IsMalformed: false);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _eventPublisherMock.Verify(e => e.PublishAsync(
            EventCategory.System,
            EventSeverity.Critical,
            It.Is<string>(s => s.Contains("kernel")),
            "Hardware fault detected on power supply",
            It.IsAny<Guid?>(),
            It.IsAny<string?>(),
            It.IsAny<Guid?>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_LowSeveritySyslog_DoesNotPublishOperationalEvent()
    {
        // Arrange
        var command = new IngestSyslogMessageCommand(
            TenantId: Guid.NewGuid(),
            SourceIpAddress: "10.0.0.5",
            Facility: SyslogFacility.User,
            Severity: SyslogSeverity.Informational,
            TimestampUtc: DateTime.UtcNow,
            Hostname: "host2",
            AppTag: "user",
            ProcessId: null,
            MessageId: null,
            Message: "User logged out",
            RawMessage: "<14>User logged out",
            IsMalformed: false);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _eventPublisherMock.Verify(e => e.PublishAsync(
            It.IsAny<EventCategory>(),
            It.IsAny<EventSeverity>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<Guid?>(),
            It.IsAny<string?>(),
            It.IsAny<Guid?>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }
}