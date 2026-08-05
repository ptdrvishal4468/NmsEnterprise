using Nms.Application.Common.Interfaces;
using Nms.Application.Devices.Commands.TestDeviceConnectivity;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;
using Nms.Domain.Models;
using Moq;
using Xunit;

namespace Nms.UnitTests;

public class TestDeviceConnectivityCommandHandlerTests
{
    private readonly Mock<IDeviceRepository> _deviceRepositoryMock = new();
    private readonly Mock<IConnectionAdapterFactory> _adapterFactoryMock = new();
    private readonly Mock<IConnectionAdapter> _adapterMock = new();

    public TestDeviceConnectivityCommandHandlerTests()
    {
        _adapterMock.Setup(a => a.Protocol).Returns(NetworkProtocol.Icmp);
        _adapterFactoryMock.Setup(f => f.GetAdapter(It.IsAny<NetworkProtocol>())).Returns(_adapterMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldThrowKeyNotFoundException_WhenDeviceDoesNotExist()
    {
        // Arrange
        var deviceId = Guid.NewGuid();
        _deviceRepositoryMock.Setup(r => r.GetByIdAsync(deviceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Device?)null);

        var handler = new TestDeviceConnectivityCommandHandler(_deviceRepositoryMock.Object, _adapterFactoryMock.Object);
        var command = new TestDeviceConnectivityCommand(deviceId, NetworkProtocol.Icmp);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldReturnConnectionResult_WhenDeviceExists()
    {
        // Arrange
        var deviceId = Guid.NewGuid();
        var device = new Device(deviceId, Guid.NewGuid(), "TestRouter", "127.0.0.1", DeviceType.Router);

        _deviceRepositoryMock.Setup(r => r.GetByIdAsync(deviceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(device);

        _adapterMock.Setup(a => a.TestConnectionAsync(It.IsAny<ConnectionParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ConnectionResult.Success(NetworkProtocol.Icmp, 12));

        var handler = new TestDeviceConnectivityCommandHandler(_deviceRepositoryMock.Object, _adapterFactoryMock.Object);
        var command = new TestDeviceConnectivityCommand(deviceId, NetworkProtocol.Icmp, 3000);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(deviceId, result.DeviceId);
        Assert.True(result.IsConnected);
        Assert.Equal(12, result.RoundTripTimeMs);
        Assert.Equal(NetworkProtocol.Icmp, result.Protocol);
    }
}