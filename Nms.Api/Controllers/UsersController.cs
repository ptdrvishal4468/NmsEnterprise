using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nms.Application.Common.Models;
using Nms.Application.Users.Commands.CreateUser;
using Nms.Application.Users.Commands.UpdateUserRoles;
using Nms.Application.Users.Dtos;
using Nms.Application.Users.Queries.GetUserById;
using Nms.Application.Users.Queries.GetUsersPaged;
using Nms.Domain.Constants;
using Nms.Infrastructure.Security;

namespace Nms.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly ISender _mediator;

    public UsersController(ISender mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new user within a tenant context.
    /// </summary>
    [HttpPost]
    [HasPermission(Permissions.Users.Create)]
    [ProducesResponseType(typeof(CreateUserResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateUser(
        [FromBody] CreateUserCommand command,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send<CreateUserResponseDto>(command, cancellationToken);
        return CreatedAtAction(nameof(GetUserById), new { id = response.UserId }, response);
    }

    /// <summary>
    /// Updates user roles.
    /// </summary>
    [HttpPut("{id:guid}/roles")]
    [HasPermission(Permissions.Users.ManageRoles)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateRoles(
        [FromRoute] Guid id,
        [FromBody] List<Guid> roleIds,
        CancellationToken cancellationToken)
    {
        var command = new UpdateUserRolesCommand(id, roleIds);
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Gets user details by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [HasPermission(Permissions.Users.View)]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetUserByIdQuery(id), cancellationToken);
        if (result == null) return NotFound();
        return Ok(result);
    }

    /// <summary>
    /// Gets a paginated list of users for a tenant.
    /// </summary>
    [HttpGet]
    [HasPermission(Permissions.Users.View)]
    [ProducesResponseType(typeof(PagedResult<UserDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsersPaged(
        [FromQuery] Guid tenantId,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetUsersPagedQuery(tenantId, pageIndex, pageSize, searchTerm);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}