using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nms.Application.Common.Models;
using Nms.Application.Locations.Buildings.Commands.CreateBuilding;
using Nms.Application.Locations.Buildings.Commands.DeleteBuilding;
using Nms.Application.Locations.Buildings.Commands.UpdateBuilding;
using Nms.Application.Locations.Buildings.Dtos;
using Nms.Application.Locations.Buildings.Queries.GetBuildingById;
using Nms.Application.Locations.Buildings.Queries.GetBuildingsBySite;
using Nms.Application.Locations.Buildings.Queries.GetBuildingsPaged;
using Nms.Domain.Constants;
using Nms.Infrastructure.Security;

namespace Nms.Api.Controllers;

[ApiController]
[Route("api/v1/buildings")]
[Authorize]
public class BuildingsController : ControllerBase
{
    private readonly ISender _sender;

    public BuildingsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [HasPermission(Permissions.Locations.View)]
    [ProducesResponseType(typeof(PagedResult<BuildingDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<BuildingDto>>> GetBuildingsPaged(
        [FromQuery] Guid? siteId = null,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? searchTerm = null,
        [FromQuery] bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetBuildingsPagedQuery(siteId, pageIndex, pageSize, searchTerm, isActive);
        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [HasPermission(Permissions.Locations.View)]
    [ProducesResponseType(typeof(BuildingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BuildingDto>> GetBuildingById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetBuildingByIdQuery(id);
        var result = await _sender.Send(query, cancellationToken);

        if (result == null)
        {
            return NotFound(new { message = $"Building with ID '{id}' was not found." });
        }

        return Ok(result);
    }

    [HttpGet("by-site/{siteId:guid}")]
    [HasPermission(Permissions.Locations.View)]
    [ProducesResponseType(typeof(IReadOnlyList<BuildingDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<BuildingDto>>> GetBuildingsBySite(
        Guid siteId,
        CancellationToken cancellationToken = default)
    {
        var query = new GetBuildingsBySiteQuery(siteId);
        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [HasPermission(Permissions.Locations.Create)]
    [ProducesResponseType(typeof(BuildingDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BuildingDto>> CreateBuilding(
        [FromBody] CreateBuildingDto dto,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateBuildingCommand(dto);
        var result = await _sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetBuildingById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [HasPermission(Permissions.Locations.Update)]
    [ProducesResponseType(typeof(BuildingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BuildingDto>> UpdateBuilding(
        Guid id,
        [FromBody] UpdateBuildingDto dto,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateBuildingCommand(id, dto);
        var result = await _sender.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [HasPermission(Permissions.Locations.Delete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteBuilding(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteBuildingCommand(id);
        var success = await _sender.Send(command, cancellationToken);

        if (!success)
        {
            return NotFound(new { message = $"Building with ID '{id}' was not found." });
        }

        return NoContent();
    }
}