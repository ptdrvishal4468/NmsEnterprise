using MediatR;
using Nms.Application.Common.Models;
using Nms.Domain.Entities;

namespace Nms.Application.Telemetry.Commands.PollDevice;

public record PollDeviceCommand(Device Device, IEnumerable<string> Oids) : IRequest<SnmpPollResult>;