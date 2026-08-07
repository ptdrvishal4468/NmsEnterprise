using MediatR;
using Nms.Application.Devices.Dtos;

namespace Nms.Application.Devices.Commands.ExecuteSshCommand;

public sealed record ExecuteSshCommand(
    Guid DeviceId,
    string Command,
    string Username,
    string? Password = null,
    string? PrivateKey = null,
    string? Passphrase = null,
    int Port = 22,
    int TimeoutSeconds = 10
) : IRequest<SshExecutionResultDto>;