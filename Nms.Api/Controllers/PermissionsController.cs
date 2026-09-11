using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nms.Application.Permissions.Commands.AssignPermissionsToRole;
using Nms.Application.Permissions.Queries.GetPermissions;
using Nms.Domain.Constants;
using Nms.Infrastructure.Security;

namespace Nms.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class PermissionsController : ControllerBase
{
    private readonly ISender _sender;

    public PermissionsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Retrieves all system permissions defined in the taxonomy.
    /// </summary>
    [HttpGet]
    [HasPermission(Permissions.Roles.View)]
    public async Task<IActionResult> GetPermissions(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetPermissionsQuery(), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Assigns or updates the collection of permissions for a specific security role.
    /// </summary>
    [HttpPost("assign")]
    [HasPermission(Permissions.Roles.AssignPermissions)]
    public async Task<IActionResult> AssignPermissionsToRole(
        [FromBody] AssignPermissionsToRoleCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return Ok(new { Success = result, Message = "Permissions assigned successfully." });
    }
}