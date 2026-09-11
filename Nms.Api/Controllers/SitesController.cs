using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nms.Application.Common.Models;
using Nms.Application.Sites.Commands.CreateSite;
using Nms.Application.Sites.Commands.DeleteSite;
using Nms.Application.Sites.Commands.UpdateSite;
using Nms.Application.Sites.Dtos;
using Nms.Application.Sites.Queries.GetSiteById;
using Nms.Application.Sites.Queries.GetSitesPaged;
using Nms.Domain.Constants;
using Nms.Infrastructure.Security;

namespace Nms.Api.Controllers;

[ApiController]
[Route("api/v1/sites")]
[Authorize]
public class SitesController : ControllerBase
{
    private readonly ISender _sender;

    public SitesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [HasPermission(Permissions.Sites.View)]
    [ProducesResponseType(typeof(PagedResult<SiteDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<SiteDto>>> GetSitesPaged(
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? searchTerm = null,
        [FromQuery] bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetSitesPagedQuery(pageIndex, pageSize, searchTerm, isActive);
        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [HasPermission(Permissions.Sites.View)]
    [ProducesResponseType(typeof(SiteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SiteDto>> GetSiteById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetSiteByIdQuery(id);
        var result = await _sender.Send(query, cancellationToken);

        if (result == null)
        {
            return NotFound(new { message = $"Site with ID '{id}' was not found." });
        }

        return Ok(result);
    }

    [HttpPost]
    [HasPermission(Permissions.Sites.Create)]
    [ProducesResponseType(typeof(SiteDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SiteDto>> CreateSite(
        [FromBody] CreateSiteDto dto,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateSiteCommand(dto);
        var result = await _sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetSiteById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [HasPermission(Permissions.Sites.Update)]
    [ProducesResponseType(typeof(SiteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SiteDto>> UpdateSite(
        Guid id,
        [FromBody] UpdateSiteDto dto,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateSiteCommand(id, dto);
        var result = await _sender.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [HasPermission(Permissions.Sites.Delete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSite(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteSiteCommand(id);
        var success = await _sender.Send(command, cancellationToken);

        if (!success)
        {
            return NotFound(new { message = $"Site with ID '{id}' was not found." });
        }

        return NoContent();
    }
}