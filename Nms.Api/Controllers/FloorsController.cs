using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nms.Application.Common.Models;
using Nms.Application.Locations.Floors.Commands.CreateFloor;
using Nms.Application.Locations.Floors.Commands.DeleteFloor;
using Nms.Application.Locations.Floors.Commands.UpdateFloor;
using Nms.Application.Locations.Floors.Dtos;
using Nms.Application.Locations.Floors.Queries.GetFloorById;
using Nms.Application.Locations.Floors.Queries.GetFloorsByBuilding;
using Nms.Application.Locations.Floors.Queries.GetFloorsPaged;
using Nms.Domain.Constants;
using Nms.Infrastructure.Security;

namespace Nms.Api.Controllers;

[ApiController]
[Route("api/v1/floors")]
[Authorize]
public class FloorsController : ControllerBase
{
    private readonly ISender _sender;

    public FloorsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [HasPermission(Permissions.Locations.View)]
    [ProducesResponseType(typeof(PagedResult<FloorDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<FloorDto>>> GetFloorsPaged(
        [FromQuery] Guid? buildingId = null,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? searchTerm = null,
        [FromQuery] bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetFloorsPagedQuery(buildingId, pageIndex, pageSize, searchTerm, isActive);
        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [HasPermission(Permissions.Locations.View)]
    [ProducesResponseType(typeof(FloorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FloorDto>> GetFloorById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetFloorByIdQuery(id);
        var result = await _sender.Send(query, cancellationToken);

        if (result == null)
        {
            return NotFound(new { message = $"Floor with ID '{id}' was not found." });
        }

        return Ok(result);
    }

    [HttpGet("by-building/{buildingId:guid}")]
    [HasPermission(Permissions.Locations.View)]
    [ProducesResponseType(typeof(IReadOnlyList<FloorDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<FloorDto>>> GetFloorsByBuilding(
        Guid buildingId,
        CancellationToken cancellationToken = default)
    {
        var query = new GetFloorsByBuildingQuery(buildingId);
        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [HasPermission(Permissions.Locations.Create)]
    [ProducesResponseType(typeof(FloorDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<FloorDto>> CreateFloor(
        [FromBody] CreateFloorDto dto,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateFloorCommand(dto);
        var result = await _sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetFloorById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [HasPermission(Permissions.Locations.Update)]
    [ProducesResponseType(typeof(FloorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FloorDto>> UpdateFloor(
        Guid id,
        [FromBody] UpdateFloorDto dto,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateFloorCommand(id, dto);
        var result = await _sender.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [HasPermission(Permissions.Locations.Delete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteFloor(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteFloorCommand(id);
        var success = await _sender.Send(command, cancellationToken);

        if (!success)
        {
            return NotFound(new { message = $"Floor with ID '{id}' was not found." });
        }

        return NoContent();
    }
}