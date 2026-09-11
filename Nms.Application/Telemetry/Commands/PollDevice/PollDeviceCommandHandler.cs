using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Common.Models;
using Nms.Domain.Interfaces;

namespace Nms.Application.Telemetry.Commands.PollDevice;

public class PollDeviceCommandHandler : IRequestHandler<PollDeviceCommand, SnmpPollResult>
{
    private readonly ISnmpCollectorService _snmpCollectorService;
    private readonly IDeviceRepository _deviceRepository;
    private readonly IOidCatalog _oidCatalog;

    public PollDeviceCommandHandler(
        ISnmpCollectorService snmpCollectorService,
        IDeviceRepository deviceRepository,
        IOidCatalog oidCatalog)
    {
        _snmpCollectorService = snmpCollectorService;
        _deviceRepository = deviceRepository;
        _oidCatalog = oidCatalog;
    }

    public async Task<SnmpPollResult> Handle(PollDeviceCommand request, CancellationToken cancellationToken)
    {
        var device = await _deviceRepository.GetByIdAsync(request.DeviceId, cancellationToken);
        if (device == null)
        {
            return new SnmpPollResult
            {
                DeviceId = request.DeviceId,
                IsSuccess = false,
                ErrorMessage = $"Device with ID '{request.DeviceId}' was not found or access is denied.",
                PolledAtUtc = DateTime.UtcNow
            };
        }

        var oidsToPoll = request.Oids?.Any() == true
            ? request.Oids
            : new[]
            {
                _oidCatalog.CiscoCpu5Min.Value,
                _oidCatalog.HostMemoryUsed.Value,
                _oidCatalog.DiskUtilization.Value,
                _oidCatalog.Temperature.Value,
                _oidCatalog.FanStatus.Value,
                _oidCatalog.PowerSupplyStatus.Value
            };

        return await _snmpCollectorService.PollDeviceAsync(device, oidsToPoll, cancellationToken);
    }
}