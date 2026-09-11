using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nms.Application.Events.Commands.RecordEvent;
using Nms.Application.Events.Queries.GetCorrelatedEvents;
using Nms.Application.Events.Queries.GetEventById;
using Nms.Application.Events.Queries.GetEventsPaged;
using Nms.Application.Events.Queries.GetEventTimeline;
using Nms.Domain.Constants;
using Nms.Domain.Enums;
using Nms.Infrastructure.Security;

namespace Nms.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class EventsController : ControllerBase
{
    private readonly ISender _sender;

    public EventsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [HasPermission(Permissions.Events.View)]
    public async Task<IActionResult> GetEvents(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] Guid? deviceId = null,
        [FromQuery] EventCategory? category = null,
        [FromQuery] EventSeverity? severity = null,
        [FromQuery] string? correlationId = null,
        [FromQuery] DateTime? fromUtc = null,
        [FromQuery] DateTime? toUtc = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetEventsPagedQuery(
            pageNumber,
            pageSize,
            deviceId,
            category,
            severity,
            correlationId,
            fromUtc,
            toUtc);

        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [HasPermission(Permissions.Events.View)]
    public async Task<IActionResult> GetEventById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetEventByIdQuery(id), cancellationToken);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPost]
    [HasPermission(Permissions.Events.Record)]
    public async Task<IActionResult> RecordEvent([FromBody] RecordEventCommand command, CancellationToken cancellationToken)
    {
        var eventId = await _sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetEventById), new { id = eventId }, new { id = eventId });
    }

    [HttpGet("device/{deviceId:guid}/timeline")]
    [HasPermission(Permissions.Events.View)]
    public async Task<IActionResult> GetDeviceEventTimeline(
        Guid deviceId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] EventCategory? category = null,
        [FromQuery] EventSeverity? severity = null,
        [FromQuery] DateTime? fromUtc = null,
        [FromQuery] DateTime? toUtc = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetEventTimelineQuery(
            deviceId,
            pageNumber,
            pageSize,
            category,
            severity,
            fromUtc,
            toUtc);

        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("correlation/{correlationId}")]
    [HasPermission(Permissions.Events.View)]
    public async Task<IActionResult> GetCorrelatedEvents(string correlationId, CancellationToken cancellationToken)
    {
        var query = new GetCorrelatedEventsQuery(correlationId);
        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }
}