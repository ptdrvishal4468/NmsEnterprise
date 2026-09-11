using Moq;
using Nms.Application.Common.Interfaces;
using Nms.Application.Devices.Commands.ExecuteSshCommand;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;
using Nms.Domain.Models;

namespace Nms.UnitTests.Ssh;

public class ExecuteSshCommandHandlerTests
{
    private readonly Mock<IDeviceRepository> _deviceRepositoryMock = new();
    private readonly Mock<ISshClientFactory> _sshClientFactoryMock = new();
    private readonly Mock<ISshClient> _sshClientMock = new();

    public ExecuteSshCommandHandlerTests()
    {
        _sshClientFactoryMock
            .Setup(f => f.CreateClient(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<SshCredentials>(), It.IsAny<TimeSpan>()))
            .Returns(_sshClientMock.Object);
    }

    [Fact]
    public async Task Handle_DeviceNotFound_ThrowsKeyNotFoundException()
    {
        _deviceRepositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Device?)null);

        var handler = new ExecuteSshCommandHandler(_deviceRepositoryMock.Object, _sshClientFactoryMock.Object);
        var command = new ExecuteSshCommand(Guid.NewGuid(), "show run", "admin", "pass");

        await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ValidDevice_ExecutesCommandSuccessfully()
    {
        var device = new Device(Guid.NewGuid(), Guid.NewGuid(), "Router-1", "192.168.1.1", DeviceType.Router);

        _deviceRepositoryMock
            .Setup(r => r.GetByIdAsync(device.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(device);

        _sshClientMock
            .Setup(c => c.ExecuteCommandAsync(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(SshExecutionResult.Success("hostname Router-1", 45));

        var handler = new ExecuteSshCommandHandler(_deviceRepositoryMock.Object, _sshClientFactoryMock.Object);
        var command = new ExecuteSshCommand(device.Id, "show run", "admin", "pass");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("hostname Router-1", result.Output);
        Assert.Equal(45, result.DurationMs);
    }
}