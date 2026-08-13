using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nms.Application.Common.Interfaces;
using Nms.Application.ConfigurationBackups.Commands.CreateConfigurationBackup;
using Nms.Application.ConfigurationBackups.Commands.PrepareRestore;
using Nms.Application.ConfigurationBackups.Dtos;
using Nms.Application.ConfigurationBackups.Queries.DownloadConfigurationBackup;
using Nms.Application.ConfigurationBackups.Queries.GetConfigurationBackupById;
using Nms.Application.ConfigurationBackups.Queries.GetConfigurationBackupsPaged;
using Nms.Domain.Constants;
using Nms.Domain.Enums;
using Nms.Infrastructure.Security;

namespace Nms.Api.Controllers;

[ApiController]
[Route("api/v1/configurationbackups")]
[Authorize]
public class ConfigurationBackupsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ITenantContext _tenantContext;

    public ConfigurationBackupsController(IMediator mediator, ITenantContext tenantContext)
    {
        _mediator = mediator;
        _tenantContext = tenantContext;
    }

    [HttpPost("devices/{deviceId:guid}")]
    [HasPermission(Permissions.ConfigurationBackup.Create)]
    public async Task<ActionResult<ConfigurationBackupDto>> TriggerBackup(
        [FromRoute] Guid deviceId,
        CancellationToken cancellationToken)
    {
        var command = new CreateConfigurationBackupCommand(_tenantContext.TenantId, deviceId, BackupTriggerType.Manual);
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet]
    [HasPermission(Permissions.ConfigurationBackup.View)]
    public async Task<ActionResult<object>> GetPaged(
        [FromQuery] Guid? deviceId,
        [FromQuery] BackupStatus? status,
        [FromQuery] BackupTriggerType? triggerType,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetConfigurationBackupsPagedQuery(_tenantContext.TenantId, deviceId, status, triggerType, pageNumber, pageSize);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [HasPermission(Permissions.ConfigurationBackup.View)]
    public async Task<ActionResult<ConfigurationBackupDto>> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetConfigurationBackupByIdQuery(_tenantContext.TenantId, id);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}/download")]
    [HasPermission(Permissions.ConfigurationBackup.Download)]
    public async Task<IActionResult> Download(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var query = new DownloadConfigurationBackupQuery(_tenantContext.TenantId, id);
        var result = await _mediator.Send(query, cancellationToken);

        var fileBytes = System.Text.Encoding.UTF8.GetBytes(result.RawConfigurationContent);
        return File(fileBytes, result.ContentType, result.FileName);
    }

    [HttpPost("{id:guid}/prepare-restore")]
    [HasPermission(Permissions.ConfigurationBackup.Create)]
    public async Task<ActionResult<RestorePreparationDto>> PrepareRestore(
        [FromRoute] Guid id,
        [FromBody] PrepareRestoreRequest? request,
        CancellationToken cancellationToken)
    {
        var command = new PrepareRestoreCommand(_tenantContext.TenantId, id, request?.Notes);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}

public record PrepareRestoreRequest(string? Notes);