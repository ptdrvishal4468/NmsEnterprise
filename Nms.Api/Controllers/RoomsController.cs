using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nms.Application.Common.Models;
using Nms.Application.Locations.Rooms.Commands.CreateRoom;
using Nms.Application.Locations.Rooms.Commands.DeleteRoom;
using Nms.Application.Locations.Rooms.Commands.UpdateRoom;
using Nms.Application.Locations.Rooms.Dtos;
using Nms.Application.Locations.Rooms.Queries.GetRoomById;
using Nms.Application.Locations.Rooms.Queries.GetRoomsByFloor;
using Nms.Application.Locations.Rooms.Queries.GetRoomsPaged;
using Nms.Domain.Constants;
using Nms.Infrastructure.Security;

namespace Nms.Api.Controllers;

[ApiController]
[Route("api/v1/rooms")]
[Authorize]
public class RoomsController : ControllerBase
{
    private readonly ISender _sender;

    public RoomsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [HasPermission(Permissions.Locations.View)]
    [ProducesResponseType(typeof(PagedResult<RoomDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<RoomDto>>> GetRoomsPaged(
        [FromQuery] Guid? floorId = null,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? searchTerm = null,
        [FromQuery] bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetRoomsPagedQuery(floorId, pageIndex, pageSize, searchTerm, isActive);
        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [HasPermission(Permissions.Locations.View)]
    [ProducesResponseType(typeof(RoomDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RoomDto>> GetRoomById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetRoomByIdQuery(id);
        var result = await _sender.Send(query, cancellationToken);

        if (result == null)
        {
            return NotFound(new { message = $"Room with ID '{id}' was not found." });
        }

        return Ok(result);
    }

    [HttpGet("by-floor/{floorId:guid}")]
    [HasPermission(Permissions.Locations.View)]
    [ProducesResponseType(typeof(IReadOnlyList<RoomDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<RoomDto>>> GetRoomsByFloor(
        Guid floorId,
        CancellationToken cancellationToken = default)
    {
        var query = new GetRoomsByFloorQuery(floorId);
        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [HasPermission(Permissions.Locations.Create)]
    [ProducesResponseType(typeof(RoomDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RoomDto>> CreateRoom(
        [FromBody] CreateRoomDto dto,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateRoomCommand(dto);
        var result = await _sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetRoomById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [HasPermission(Permissions.Locations.Update)]
    [ProducesResponseType(typeof(RoomDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RoomDto>> UpdateRoom(
        Guid id,
        [FromBody] UpdateRoomDto dto,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateRoomCommand(id, dto);
        var result = await _sender.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [HasPermission(Permissions.Locations.Delete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteRoom(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteRoomCommand(id);
        var success = await _sender.Send(command, cancellationToken);

        if (!success)
        {
            return NotFound(new { message = $"Room with ID '{id}' was not found." });
        }

        return NoContent();
    }
}