using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nms.Application.Common.Models;
using Nms.Application.Reachability.Commands.PingDevice;
using Nms.Application.Reachability.Dtos;
using Nms.Application.Reachability.Queries.GetDeviceCurrentStatus;
using Nms.Application.Reachability.Queries.GetDeviceReachabilityHistory;
using Nms.Domain.Constants;
using Nms.Infrastructure.Security;

namespace Nms.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ReachabilityController : ControllerBase
{
    private readonly ISender _mediator;

    public ReachabilityController(ISender mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Executes an on-demand ICMP ping check against a specified device.
    /// </summary>
    [HttpPost("devices/{deviceId:guid}/ping")]
    [HasPermission(Permissions.Devices.View)]
    public async Task<ActionResult<PingSummaryDto>> PingDevice(
        Guid deviceId,
        [FromQuery] int count = 4,
        [FromQuery] int timeoutMs = 1000,
        CancellationToken cancellationToken = default)
    {
        var command = new PingDeviceCommand(deviceId, count, timeoutMs);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Fetches the current/latest reachability status and latency summary for a device.
    /// </summary>
    [HttpGet("devices/{deviceId:guid}/current")]
    [HasPermission(Permissions.Devices.View)]
    public async Task<ActionResult<PingSummaryDto>> GetCurrentStatus(
        Guid deviceId,
        CancellationToken cancellationToken = default)
    {
        var query = new GetDeviceCurrentStatusQuery(deviceId);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Fetches paged reachability execution history for a device.
    /// </summary>
    [HttpGet("devices/{deviceId:guid}/history")]
    [HasPermission(Permissions.Devices.View)]
    public async Task<ActionResult<PagedResult<ReachabilityHistoryDto>>> GetReachabilityHistory(
        Guid deviceId,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetDeviceReachabilityHistoryQuery(deviceId, pageIndex, pageSize);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}