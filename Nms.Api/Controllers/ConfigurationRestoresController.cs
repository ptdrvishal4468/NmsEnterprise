using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nms.Application.Common.Models;
using Nms.Application.ConfigurationBackups.Commands.RestoreConfiguration;
using Nms.Application.ConfigurationBackups.Dtos;
using Nms.Application.ConfigurationBackups.Queries.GetRestoreLogById;
using Nms.Application.ConfigurationBackups.Queries.GetRestoreLogsPaged;
using Nms.Domain.Constants;
using Nms.Infrastructure.Security;

namespace Nms.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ConfigurationRestoresController : ControllerBase
{
    private readonly IMediator _mediator;

    public ConfigurationRestoresController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Triggers a configuration restore operation for a specific backup snapshot.
    /// </summary>
    [HttpPost("backups/{backupId:guid}")]
    [HasPermission(Permissions.ConfigurationBackup.Restore)]
    [ProducesResponseType(typeof(RestoreExecutionResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RestoreConfiguration(
        [FromRoute] Guid backupId,
        CancellationToken cancellationToken)
    {
        var command = new RestoreConfigurationCommand(backupId);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a paged list of configuration restore execution audit logs.
    /// </summary>
    [HttpGet("logs")]
    [HasPermission(Permissions.ConfigurationBackup.View)]
    [ProducesResponseType(typeof(PagedResult<ConfigurationRestoreLogDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRestoreLogs(
        [FromQuery] Guid? deviceId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetRestoreLogsPagedQuery(deviceId, pageNumber, pageSize);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a specific configuration restore execution audit log by ID.
    /// </summary>
    [HttpGet("logs/{id:guid}")]
    [HasPermission(Permissions.ConfigurationBackup.View)]
    [ProducesResponseType(typeof(ConfigurationRestoreLogDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRestoreLogById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetRestoreLogByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (result == null)
        {
            return NotFound(new { Message = $"Configuration restore log with ID '{id}' was not found." });
        }

        return Ok(result);
    }
}