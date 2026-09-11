using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Devices.Dtos;
using Nms.Domain.Interfaces;
using Nms.Domain.Models;

namespace Nms.Application.Devices.Commands.ExecuteSshCommand;

public sealed class ExecuteSshCommandHandler : IRequestHandler<ExecuteSshCommand, SshExecutionResultDto>
{
    private readonly IDeviceRepository _deviceRepository;
    private readonly ISshClientFactory _sshClientFactory;

    public ExecuteSshCommandHandler(
        IDeviceRepository deviceRepository,
        ISshClientFactory sshClientFactory)
    {
        _deviceRepository = deviceRepository ?? throw new ArgumentNullException(nameof(deviceRepository));
        _sshClientFactory = sshClientFactory ?? throw new ArgumentNullException(nameof(sshClientFactory));
    }

    public async Task<SshExecutionResultDto> Handle(ExecuteSshCommand request, CancellationToken cancellationToken)
    {
        var device = await _deviceRepository.GetByIdAsync(request.DeviceId, cancellationToken);
        if (device == null)
        {
            throw new KeyNotFoundException($"Device with ID '{request.DeviceId}' was not found.");
        }

        SshCredentials credentials = !string.IsNullOrWhiteSpace(request.PrivateKey)
            ? SshCredentials.FromPrivateKey(request.Username, request.PrivateKey, request.Passphrase)
            : SshCredentials.FromPassword(request.Username, request.Password!);

        var timeout = TimeSpan.FromSeconds(request.TimeoutSeconds);

        await using var client = _sshClientFactory.CreateClient(
            device.IpAddress,
            request.Port,
            credentials,
            timeout);

        var result = await client.ExecuteCommandAsync(request.Command, timeout, cancellationToken);

        return new SshExecutionResultDto(
            result.IsSuccess,
            result.ExitCode,
            result.Output,
            result.ErrorOutput,
            result.DurationMs
        );
    }
}