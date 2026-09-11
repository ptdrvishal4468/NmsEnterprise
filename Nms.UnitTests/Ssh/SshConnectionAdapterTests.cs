using Moq;
using Nms.Application.Common.Interfaces;
using Nms.Domain.Enums;
using Nms.Domain.Models;
using Nms.Infrastructure.Connectivity;

namespace Nms.UnitTests.Ssh;

public class SshConnectionAdapterTests
{
    private readonly Mock<ISshClientFactory> _factoryMock = new();
    private readonly Mock<ISshClient> _clientMock = new();

    public SshConnectionAdapterTests()
    {
        _factoryMock
            .Setup(f => f.CreateClient(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<SshCredentials>(), It.IsAny<TimeSpan>()))
            .Returns(_clientMock.Object);
    }

    [Fact]
    public async Task TestConnectionAsync_SuccessfulHandshake_ReturnsSuccess()
    {
        _clientMock
            .Setup(c => c.TestConnectionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var adapter = new SshConnectionAdapter(_factoryMock.Object);
        var paramsObj = new ConnectionParameters
        {
            HostOrIp = "192.168.1.1",
            Port = 22,
            Protocol = NetworkProtocol.Ssh,
            Username = "admin",
            Password = "password"
        };

        var result = await adapter.TestConnectionAsync(paramsObj, CancellationToken.None);

        Assert.True(result.IsConnected);
        Assert.Equal(NetworkProtocol.Ssh, result.Protocol);
    }
}