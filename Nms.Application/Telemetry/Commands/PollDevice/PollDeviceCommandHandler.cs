using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Common.Models;

namespace Nms.Application.Telemetry.Commands.PollDevice;

public class PollDeviceCommandHandler : IRequestHandler<PollDeviceCommand, SnmpPollResult>
{
    private readonly ISnmpCollectorService _snmpCollectorService;

    public PollDeviceCommandHandler(ISnmpCollectorService snmpCollectorService)
    {
        _snmpCollectorService = snmpCollectorService;
    }

    public async Task<SnmpPollResult> Handle(PollDeviceCommand request, CancellationToken cancellationToken)
    {
        return await _snmpCollectorService.PollDeviceAsync(request.Device, request.Oids, cancellationToken);
    }
}