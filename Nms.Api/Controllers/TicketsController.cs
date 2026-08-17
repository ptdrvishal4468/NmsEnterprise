using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nms.Application.Common.Models;
using Nms.Application.Ticketing.Commands.CloseTicket;
using Nms.Application.Ticketing.Commands.CreateTicket;
using Nms.Application.Ticketing.Commands.SyncTicket;
using Nms.Application.Ticketing.Commands.UpdateTicket;
using Nms.Application.Ticketing.Dtos;
using Nms.Application.Ticketing.Queries.GetTicketById;
using Nms.Application.Ticketing.Queries.GetTicketsPaged;
using Nms.Application.Ticketing.Queries.GetTicketSyncLogs;
using Nms.Domain.Constants;
using Nms.Domain.Enums;
using Nms.Infrastructure.Security;

namespace Nms.Api.Controllers;

[ApiController]
[Route("api/v1/tickets")]
[Authorize]
public class TicketsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TicketsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [HasPermission(Permissions.Ticketing.View)]
    [ProducesResponseType(typeof(PagedResult<TicketDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTicketsPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? searchTerm = null,
        [FromQuery] TicketStatus? status = null,
        [FromQuery] TicketPriority? priority = null,
        [FromQuery] TicketingProviderType? providerType = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetTicketsPagedQuery(pageNumber, pageSize, searchTerm, status, priority, providerType);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [HasPermission(Permissions.Ticketing.View)]
    [ProducesResponseType(typeof(TicketDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTicketById(Guid id, CancellationToken cancellationToken = default)
    {
        var query = new GetTicketByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (result == null)
            return NotFound(new { message = $"Ticket with ID {id} not found." });

        return Ok(result);
    }

    [HttpPost]
    [HasPermission(Permissions.Ticketing.Create)]
    [ProducesResponseType(typeof(TicketDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTicket([FromBody] CreateTicketDto dto, CancellationToken cancellationToken = default)
    {
        var command = new CreateTicketCommand(
            dto.Title,
            dto.Description,
            dto.Priority,
            dto.ProviderType,
            dto.DeviceId,
            dto.AlertId,
            dto.CustomerId,
            dto.MetadataJson,
            dto.DispatchToProvider);

        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetTicketById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [HasPermission(Permissions.Ticketing.Update)]
    [ProducesResponseType(typeof(TicketDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateTicket(Guid id, [FromBody] UpdateTicketDto dto, CancellationToken cancellationToken = default)
    {
        var command = new UpdateTicketCommand(id, dto.Title, dto.Description, dto.Priority, dto.Status, dto.MetadataJson);
        var result = await _mediator.Send(command, cancellationToken);

        if (result == null)
            return NotFound(new { message = $"Ticket with ID {id} not found." });

        return Ok(result);
    }

    [HttpPost("{id:guid}/close")]
    [HasPermission(Permissions.Ticketing.Update)]
    [ProducesResponseType(typeof(TicketDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CloseTicket(Guid id, CancellationToken cancellationToken = default)
    {
        var command = new CloseTicketCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        if (result == null)
            return NotFound(new { message = $"Ticket with ID {id} not found." });

        return Ok(result);
    }

    [HttpPost("{id:guid}/sync")]
    [HasPermission(Permissions.Ticketing.Sync)]
    [ProducesResponseType(typeof(SyncTicketResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SyncTicket(Guid id, CancellationToken cancellationToken = default)
    {
        var command = new SyncTicketCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        if (result == null)
            return NotFound(new { message = $"Ticket with ID {id} not found." });

        return Ok(result);
    }

    [HttpGet("{id:guid}/sync-logs")]
    [HasPermission(Permissions.Ticketing.View)]
    [ProducesResponseType(typeof(IReadOnlyList<TicketSyncLogDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTicketSyncLogs(Guid id, CancellationToken cancellationToken = default)
    {
        var query = new GetTicketSyncLogsQuery(id);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}