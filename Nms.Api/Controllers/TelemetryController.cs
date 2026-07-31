using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nms.Application.Common.Models;
using Nms.Application.Telemetry.Commands.PollDevice;
using Nms.Application.Telemetry.Commands.ProcessTelemetryData;
using Nms.Application.Telemetry.Dtos;
using Nms.Application.Telemetry.Queries.GetDeviceMetrics;
using Nms.Domain.Entities;

namespace Nms.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TelemetryController : ControllerBase
{
    private readonly ISender _mediator;

    public TelemetryController(ISender mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Manually triggers an SNMP poll for a given device and OID list.
    /// </summary>
    [HttpPost("poll")]
    [ProducesResponseType(typeof(SnmpPollResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> PollDevice([FromBody] PollDeviceCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Processes and persists raw SNMP poll data into telemetry metrics.
    /// </summary>
    [HttpPost("process")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> ProcessTelemetry([FromBody] ProcessTelemetryDataCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Fetches historical device metrics within a specified UTC date range.
    /// </summary>
    [HttpGet("devices/{deviceId:guid}/metrics")]
    [ProducesResponseType(typeof(IEnumerable<DeviceMetricDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDeviceMetrics(
        [FromRoute] Guid deviceId,
        [FromQuery] DateTime fromUtc,
        [FromQuery] DateTime toUtc,
        CancellationToken cancellationToken)
    {
        var query = new GetDeviceMetricsQuery(deviceId, fromUtc, toUtc);
        var metrics = await _mediator.Send(query, cancellationToken);
        return Ok(metrics);
    }
}