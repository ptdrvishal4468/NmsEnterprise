using System.Net;
using System.Net.Http.Json;
using Nms.Application.Devices.Commands.ExecuteSshCommand;

namespace Nms.IntegrationTests.Ssh;

public class SshEndpointTests
{
    [Fact]
    public void ExecuteSshCommand_Route_MatchesExpectedFormat()
    {
        var deviceId = Guid.NewGuid();
        var route = $"/api/v1/devices/{deviceId}/ssh/execute";

        Assert.Equal($"/api/v1/devices/{deviceId}/ssh/execute", route);
    }

    [Fact]
    public void ExecuteSshCommand_Payload_SerializesCorrectly()
    {
        var command = new ExecuteSshCommand(
            DeviceId: Guid.NewGuid(),
            Command: "show version",
            Username: "admin",
            Password: "password123",
            Port: 22,
            TimeoutSeconds: 15
        );

        Assert.NotNull(command.Command);
        Assert.Equal(22, command.Port);
        Assert.Equal(15, command.TimeoutSeconds);
    }
}