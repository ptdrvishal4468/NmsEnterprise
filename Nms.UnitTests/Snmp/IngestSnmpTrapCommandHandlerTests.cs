using Moq;
using Microsoft.Extensions.Logging;
using Nms.Application.Common.Interfaces;
using Nms.Application.SnmpTraps.Commands.IngestSnmpTrap;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;
using Xunit;

namespace Nms.UnitTests.Snmp;

public class IngestSnmpTrapCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IEventPublisher> _eventPublisherMock = new();
    private readonly Mock<ILogger<IngestSnmpTrapCommandHandler>> _loggerMock = new();

    [Fact]
    public async Task Handle_CriticalTrap_PersistsEntityAndPublishesSystemEvent()
    {
        var deviceRepoMock = new Mock<IDeviceRepository>();
        var trapRepoMock = new Mock<ISnmpTrapRepository>();

        var deviceId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();

        var testDevice = new Device(
            id: deviceId,
            tenantId: tenantId,
            name: "Core Switch",
            ipAddress: "10.0.0.1",
            deviceType: DeviceType.Switch);

        deviceRepoMock.Setup(x => x.GetByIpAddressAsync("10.0.0.1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(testDevice);

        _unitOfWorkMock.Setup(x => x.Devices).Returns(deviceRepoMock.Object);
        _unitOfWorkMock.Setup(x => x.SnmpTraps).Returns(trapRepoMock.Object);

        var handler = new IngestSnmpTrapCommandHandler(
            _unitOfWorkMock.Object,
            _eventPublisherMock.Object,
            _loggerMock.Object);

        var command = new IngestSnmpTrapCommand(
            Version: SnmpVersion.V2c,
            EnterpriseOid: "1.3.6.1.4.1.9",
            SourceIpAddress: "10.0.0.1",
            Severity: TrapSeverity.Critical,
            VarbindsJson: "[]",
            TimestampUtc: DateTime.UtcNow);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotEqual(Guid.Empty, result);
        trapRepoMock.Verify(x => x.AddAsync(It.IsAny<SnmpTrapMessage>(), It.IsAny<CancellationToken>()), Times.Once);
        _eventPublisherMock.Verify(x => x.PublishAsync(
            EventCategory.System,
            EventSeverity.Critical,
            It.IsAny<string>(),
            It.IsAny<string>(),
            testDevice.Id,
            It.IsAny<string>(),
            It.IsAny<Guid?>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}