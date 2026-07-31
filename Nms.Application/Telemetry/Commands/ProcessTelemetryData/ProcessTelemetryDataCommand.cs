using MediatR;
using Nms.Application.Common.Models;

namespace Nms.Application.Telemetry.Commands.ProcessTelemetryData;

public record ProcessTelemetryDataCommand(SnmpPollResult PollResult) : IRequest<bool>;