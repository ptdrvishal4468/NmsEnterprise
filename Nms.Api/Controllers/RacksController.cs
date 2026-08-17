using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nms.Application.Common.Models;
using Nms.Application.Locations.Racks.Commands.CreateRack;
using Nms.Application.Locations.Racks.Commands.DeleteRack;
using Nms.Application.Locations.Racks.Commands.UpdateRack;
using Nms.Application.Locations.Racks.Dtos;
using Nms.Application.Locations.Racks.Queries.GetRackById;
using Nms.Application.Locations.Racks.Queries.GetRacksByRoom;
using Nms.Application.Locations.Racks.Queries.GetRacksPaged;
using Nms.Domain.Constants;
using Nms.Infrastructure.Security;

namespace Nms.Api.Controllers;

[ApiController]
[Route("api/v1/racks")]
[Authorize]
public class RacksController : ControllerBase
{
    private readonly ISender _sender;

    public RacksController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [HasPermission(Permissions.Locations.View)]
    [ProducesResponseType(typeof(PagedResult<RackDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<RackDto>>> GetRacksPaged(
        [FromQuery] Guid? roomId = null,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? searchTerm = null,
        [FromQuery] bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetRacksPagedQuery(roomId, pageIndex, pageSize, searchTerm, isActive);
        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [HasPermission(Permissions.Locations.View)]
    [ProducesResponseType(typeof(RackDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RackDto>> GetRackById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetRackByIdQuery(id);
        var result = await _sender.Send(query, cancellationToken);

        if (result == null)
        {
            return NotFound(new { message = $"Rack with ID '{id}' was not found." });
        }

        return Ok(result);
    }

    [HttpGet("by-room/{roomId:guid}")]
    [HasPermission(Permissions.Locations.View)]
    [ProducesResponseType(typeof(IReadOnlyList<RackDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<RackDto>>> GetRacksByRoom(
        Guid roomId,
        CancellationToken cancellationToken = default)
    {
        var query = new GetRacksByRoomQuery(roomId);
        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [HasPermission(Permissions.Locations.Create)]
    [ProducesResponseType(typeof(RackDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RackDto>> CreateRack(
        [FromBody] CreateRackDto dto,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateRackCommand(dto);
        var result = await _sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetRackById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [HasPermission(Permissions.Locations.Update)]
    [ProducesResponseType(typeof(RackDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RackDto>> UpdateRack(
        Guid id,
        [FromBody] UpdateRackDto dto,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateRackCommand(id, dto);
        var result = await _sender.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [HasPermission(Permissions.Locations.Delete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteRack(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteRackCommand(id);
        var success = await _sender.Send(command, cancellationToken);

        if (!success)
        {
            return NotFound(new { message = $"Rack with ID '{id}' was not found." });
        }

        return NoContent();
    }
}