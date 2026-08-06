using Moq;
using Nms.Application.Common.Interfaces;
using Nms.Application.Reachability.Commands.PingDevice;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;
using Nms.Domain.Models;
using Xunit;

namespace Nms.UnitTests.Reachability;

public class PingDeviceCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IIcmpPingService> _icmpPingServiceMock;
    private readonly Mock<IDeviceRepository> _deviceRepoMock;
    private readonly Mock<IReachabilityHistoryRepository> _historyRepoMock;
    private readonly PingDeviceCommandHandler _handler;

    public PingDeviceCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _icmpPingServiceMock = new Mock<IIcmpPingService>();
        _deviceRepoMock = new Mock<IDeviceRepository>();
        _historyRepoMock = new Mock<IReachabilityHistoryRepository>();

        _unitOfWorkMock.Setup(u => u.Devices).Returns(_deviceRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.ReachabilityHistories).Returns(_historyRepoMock.Object);

        _handler = new PingDeviceCommandHandler(_unitOfWorkMock.Object, _icmpPingServiceMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Throw_KeyNotFoundException_When_Device_DoesNotExist()
    {
        var deviceId = Guid.NewGuid();
        _deviceRepoMock
            .Setup(r => r.GetByIdAsync(deviceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Device?)null);

        var command = new PingDeviceCommand(deviceId);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Should_Update_Device_Status_And_Persist_History_When_Successful()
    {
        var deviceId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var device = new Device(
            deviceId,
            tenantId,
            "Core-Router-01",
            "192.168.1.1",
            DeviceType.Router);

        var pingResult = new PingResult(
            ReachabilityStatus.Online,
            minLatencyMs: 10,
            maxLatencyMs: 25,
            avgLatencyMs: 15,
            currentLatencyMs: 12,
            packetsSent: 4,
            packetsReceived: 4,
            packetLossPercentage: 0);

        _deviceRepoMock
            .Setup(r => r.GetByIdAsync(deviceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(device);

        _icmpPingServiceMock
            .Setup(s => s.PingAsync("192.168.1.1", 4, 1000, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pingResult);

        var command = new PingDeviceCommand(deviceId, 4, 1000);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(ReachabilityStatus.Online, result.Status);
        Assert.Equal(DeviceStatus.Online, device.Status);

        _historyRepoMock.Verify(h => h.AddAsync(It.Is<DeviceReachabilityHistory>(
            x => x.DeviceId == deviceId && x.Status == ReachabilityStatus.Online
        ), It.IsAny<CancellationToken>()), Times.Once);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}