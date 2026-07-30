using Microsoft.AspNetCore.Mvc;
using Nms.Application.Common.Models;
using Nms.Application.Users.Commands.CreateUser;
using Nms.Application.Users.Commands.UpdateUserRoles;
using Nms.Application.Users.Dtos;
using Nms.Application.Users.Queries.GetUserById;
using Nms.Application.Users.Queries.GetUsersPaged;

namespace Nms.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class UsersController : ControllerBase
{
    private readonly CreateUserCommandHandler _createUserHandler;
    private readonly UpdateUserRolesCommandHandler _updateUserRolesHandler;
    private readonly GetUserByIdQueryHandler _getUserByIdHandler;
    private readonly GetUsersPagedQueryHandler _getUsersPagedHandler;

    public UsersController(
        CreateUserCommandHandler createUserHandler,
        UpdateUserRolesCommandHandler updateUserRolesHandler,
        GetUserByIdQueryHandler getUserByIdHandler,
        GetUsersPagedQueryHandler getUsersPagedHandler)
    {
        _createUserHandler = createUserHandler;
        _updateUserRolesHandler = updateUserRolesHandler;
        _getUserByIdHandler = getUserByIdHandler;
        _getUsersPagedHandler = getUsersPagedHandler;
    }

    /// <summary>
    /// Creates a new user within a tenant context.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CreateUserResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateUser(
        [FromBody] CreateUserCommand command,
        CancellationToken cancellationToken)
    {
        var response = await _createUserHandler.HandleAsync(command, cancellationToken);
        return CreatedAtAction(nameof(GetUserById), new { id = response.UserId }, response);
    }

    /// <summary>
    /// Updates user roles.
    /// </summary>
    [HttpPut("{id:guid}/roles")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateRoles(
        [FromRoute] Guid id,
        [FromBody] List<Guid> roleIds,
        CancellationToken cancellationToken)
    {
        var command = new UpdateUserRolesCommand(id, roleIds);
        await _updateUserRolesHandler.HandleAsync(command, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Gets user details by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _getUserByIdHandler.HandleAsync(new GetUserByIdQuery(id), cancellationToken);
        if (result == null) return NotFound();
        return Ok(result);
    }

    /// <summary>
    /// Gets a paginated list of users for a tenant.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<UserDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsersPaged(
        [FromQuery] Guid tenantId,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetUsersPagedQuery(tenantId, pageIndex, pageSize, searchTerm);
        var result = await _getUsersPagedHandler.HandleAsync(query, cancellationToken);
        return Ok(result);
    }
}