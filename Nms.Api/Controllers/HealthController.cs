using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nms.Application.Common.Models;
using Nms.Application.Health.Commands.EvaluateDeviceHealth;
using Nms.Application.Health.Dtos;
using Nms.Application.Health.Queries.GetDeviceHealth;
using Nms.Application.Health.Queries.GetDeviceHealthHistory;
using Nms.Domain.Constants;
using Nms.Infrastructure.Security;

namespace Nms.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class HealthController : ControllerBase
{
    private readonly ISender _mediator;

    public HealthController(ISender mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Fetches the current health evaluation for a specific device.
    /// </summary>
    [HttpGet("devices/{deviceId:guid}")]
    [HasPermission(Permissions.Health.View)]
    public async Task<ActionResult<DeviceHealthDto>> GetDeviceHealth(
        Guid deviceId,
        CancellationToken cancellationToken = default)
    {
        var query = new GetDeviceHealthQuery(deviceId);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Fetches paged historical health evaluation logs for a specific device.
    /// </summary>
    [HttpGet("devices/{deviceId:guid}/history")]
    [HasPermission(Permissions.Health.History)]
    public async Task<ActionResult<PagedResult<DeviceHealthHistoryDto>>> GetDeviceHealthHistory(
        Guid deviceId,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetDeviceHealthHistoryQuery(deviceId, pageIndex, pageSize);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Triggers an on-demand health evaluation for a device and records a historical snapshot.
    /// </summary>
    [HttpPost("devices/{deviceId:guid}/evaluate")]
    [HasPermission(Permissions.Health.Evaluate)]
    public async Task<ActionResult<DeviceHealthDto>> EvaluateDeviceHealth(
        Guid deviceId,
        CancellationToken cancellationToken = default)
    {
        var command = new EvaluateDeviceHealthCommand(deviceId);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}