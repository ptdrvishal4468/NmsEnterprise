using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Devices.Dtos;
using Nms.Domain.Interfaces;
using Nms.Domain.Models;

namespace Nms.Application.Devices.Commands.TestDeviceConnectivity;

public sealed class TestDeviceConnectivityCommandHandler : IRequestHandler<TestDeviceConnectivityCommand, DeviceConnectionResultDto>
{
    private readonly IDeviceRepository _deviceRepository;
    private readonly IConnectionAdapterFactory _adapterFactory;

    public TestDeviceConnectivityCommandHandler(
        IDeviceRepository deviceRepository,
        IConnectionAdapterFactory adapterFactory)
    {
        _deviceRepository = deviceRepository ?? throw new ArgumentNullException(nameof(deviceRepository));
        _adapterFactory = adapterFactory ?? throw new ArgumentNullException(nameof(adapterFactory));
    }

    public async Task<DeviceConnectionResultDto> Handle(TestDeviceConnectivityCommand request, CancellationToken cancellationToken)
    {
        var device = await _deviceRepository.GetByIdAsync(request.DeviceId, cancellationToken);
        if (device == null)
        {
            throw new KeyNotFoundException($"Device with ID '{request.DeviceId}' was not found.");
        }

        var parameters = new ConnectionParameters
        {
            HostOrIp = device.IpAddress,
            Port = device.SnmpPort,
            Protocol = request.Protocol,
            Timeout = TimeSpan.FromMilliseconds(request.TimeoutMs)
        };

        var adapter = _adapterFactory.GetAdapter(request.Protocol);
        var result = await adapter.TestConnectionAsync(parameters, cancellationToken);

        return new DeviceConnectionResultDto
        {
            DeviceId = device.Id,
            Protocol = result.Protocol,
            IsConnected = result.IsConnected,
            RoundTripTimeMs = result.RoundTripTimeMs,
            ErrorMessage = result.ErrorMessage,
            TestedAtUtc = result.TestedAtUtc
        };
    }
}