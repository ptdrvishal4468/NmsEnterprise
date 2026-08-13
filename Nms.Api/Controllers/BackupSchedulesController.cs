using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nms.Application.Common.Interfaces;
using Nms.Application.ConfigurationBackups.Commands.CreateBackupSchedule;
using Nms.Application.ConfigurationBackups.Dtos;
using Nms.Application.ConfigurationBackups.Queries.GetBackupSchedulesPaged;
using Nms.Domain.Constants;
using Nms.Infrastructure.Security;

namespace Nms.Api.Controllers;

[ApiController]
[Route("api/v1/backupschedules")]
[Authorize]
public class BackupSchedulesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ITenantContext _tenantContext;

    public BackupSchedulesController(IMediator mediator, ITenantContext tenantContext)
    {
        _mediator = mediator;
        _tenantContext = tenantContext;
    }

    [HttpPost]
    [HasPermission(Permissions.ConfigurationBackup.Schedule)]
    public async Task<ActionResult<BackupScheduleDto>> CreateSchedule(
        [FromBody] CreateBackupScheduleRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateBackupScheduleCommand(
            _tenantContext.TenantId,
            request.Name,
            request.IntervalMinutes,
            request.DeviceId,
            request.Description);

        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetPaged), new { id = result.Id }, result);
    }

    [HttpGet]
    [HasPermission(Permissions.ConfigurationBackup.View)]
    public async Task<ActionResult<object>> GetPaged(
        [FromQuery] Guid? deviceId,
        [FromQuery] bool? isEnabled,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetBackupSchedulesPagedQuery(_tenantContext.TenantId, deviceId, isEnabled, pageNumber, pageSize);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}

public record CreateBackupScheduleRequest(
    string Name,
    int IntervalMinutes,
    Guid? DeviceId = null,
    string? Description = null);