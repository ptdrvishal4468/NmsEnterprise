using MediatR;
using Nms.Application.Common.Models;

namespace Nms.Application.Telemetry.Commands.PollDevice;

public record PollDeviceCommand(
    Guid DeviceId,
    IEnumerable<string>? Oids = null
) : IRequest<SnmpPollResult>;