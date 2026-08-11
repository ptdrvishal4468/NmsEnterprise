using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nms.Application.Alerts.Commands.AcknowledgeAlert;
using Nms.Application.Alerts.Commands.SuppressAlert;
using Nms.Application.Alerts.Dtos;
using Nms.Application.Alerts.Queries.GetAlertById;
using Nms.Application.Alerts.Queries.GetAlertsPaged;
using Nms.Application.Common.Models;
using Nms.Domain.Constants;
using Nms.Domain.Enums;
using Nms.Infrastructure.Security;

namespace Nms.Api.Controllers;

[ApiController]
[Route("api/v1/alerts")]
[Authorize]
public class AlertsController : ControllerBase
{
    private readonly ISender _mediator;

    public AlertsController(ISender mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Fetches a paged list of alerts filtered by state, severity, or device.
    /// </summary>
    [HttpGet]
    [HasPermission(Permissions.Alerts.View)]
    public async Task<ActionResult<PagedResult<AlertDto>>> GetAlerts(
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] Guid? deviceId = null,
        [FromQuery] AlertState? state = null,
        [FromQuery] AlertSeverity? severity = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAlertsPagedQuery(pageIndex, pageSize, deviceId, state, severity);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Fetches detailed information for a specific alert by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [HasPermission(Permissions.Alerts.View)]
    public async Task<ActionResult<AlertDto>> GetAlertById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAlertByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    /// <summary>
    /// Acknowledges an active alert.
    /// </summary>
    [HttpPost("{id:guid}/acknowledge")]
    [HasPermission(Permissions.Alerts.Acknowledge)]
    public async Task<ActionResult<AlertDto>> AcknowledgeAlert(
        Guid id,
        [FromBody] AcknowledgeAlertRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = User.Identity?.Name ?? "SystemUser";
        var command = new AcknowledgeAlertCommand(id, user, request.Note);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Suppresses an active alert.
    /// </summary>
    [HttpPost("{id:guid}/suppress")]
    [HasPermission(Permissions.Alerts.Suppress)]
    public async Task<ActionResult<AlertDto>> SuppressAlert(
        Guid id,
        [FromBody] SuppressAlertRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = User.Identity?.Name ?? "SystemUser";
        var command = new SuppressAlertCommand(id, user, request.Note);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}

public record AcknowledgeAlertRequest(string? Note);
public record SuppressAlertRequest(string? Note);