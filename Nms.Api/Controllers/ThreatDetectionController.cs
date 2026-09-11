using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nms.Application.Common.Models;
using Nms.Application.ThreatDetection.Commands.AnalyzeConfigurationDrift;
using Nms.Application.ThreatDetection.Commands.AnalyzeFailedLogins;
using Nms.Application.ThreatDetection.Commands.AnalyzePortScans;
using Nms.Application.ThreatDetection.Commands.AnalyzeUnauthorizedAccess;
using Nms.Application.ThreatDetection.Commands.CreateThreatRule;
using Nms.Application.ThreatDetection.Commands.UpdateThreatIndicatorStatus;
using Nms.Application.ThreatDetection.Dtos;
using Nms.Application.ThreatDetection.Queries.GetConfigurationDriftsPaged;
using Nms.Application.ThreatDetection.Queries.GetThreatIndicatorById;
using Nms.Application.ThreatDetection.Queries.GetThreatIndicatorsPaged;
using Nms.Application.ThreatDetection.Queries.GetThreatRulesPaged;
using Nms.Application.ThreatDetection.Queries.GetThreatSummary;
using Nms.Domain.Constants;
using Nms.Domain.Enums;
using Nms.Infrastructure.Security;

namespace Nms.Api.Controllers;

[ApiController]
[Route("api/v1/threats")]
[Authorize]
public class ThreatDetectionController : ControllerBase
{
    private readonly IMediator _mediator;

    public ThreatDetectionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("indicators")]
    [HasPermission(Permissions.ThreatDetection.View)]
    public async Task<ActionResult<PagedResult<ThreatIndicatorDto>>> GetIndicators(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] ThreatType? threatType = null,
        [FromQuery] ThreatSeverity? severity = null,
        [FromQuery] ThreatStatus? status = null,
        [FromQuery] Guid? deviceId = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetThreatIndicatorsPagedQuery(pageNumber, pageSize, threatType, severity, status, deviceId),
            cancellationToken);
        return Ok(result);
    }

    [HttpGet("indicators/{id:guid}")]
    [HasPermission(Permissions.ThreatDetection.View)]
    public async Task<ActionResult<ThreatIndicatorDto>> GetIndicatorById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetThreatIndicatorByIdQuery(id), cancellationToken);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPut("indicators/{id:guid}/status")]
    [HasPermission(Permissions.ThreatDetection.Manage)]
    public async Task<ActionResult<ThreatIndicatorDto>> UpdateIndicatorStatus(
        Guid id,
        [FromBody] UpdateThreatIndicatorStatusDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new UpdateThreatIndicatorStatusCommand(id, dto.Status, dto.ResolutionNotes),
            cancellationToken);
        return Ok(result);
    }

    [HttpPost("analyze/failed-logins")]
    [HasPermission(Permissions.ThreatDetection.Analyze)]
    public async Task<ActionResult<IReadOnlyList<ThreatIndicatorDto>>> AnalyzeFailedLogins(
        [FromQuery] int? threshold = null,
        [FromQuery] int? timeWindowMinutes = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new AnalyzeFailedLoginsCommand(threshold, timeWindowMinutes), cancellationToken);
        return Ok(result);
    }

    [HttpPost("analyze/config-drift")]
    [HasPermission(Permissions.ThreatDetection.Analyze)]
    public async Task<ActionResult<IReadOnlyList<ConfigurationDriftRecordDto>>> AnalyzeConfigDrift(
        [FromQuery] Guid? deviceId = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new AnalyzeConfigurationDriftCommand(deviceId), cancellationToken);
        return Ok(result);
    }

    [HttpGet("config-drifts")]
    [HasPermission(Permissions.ThreatDetection.View)]
    public async Task<ActionResult<PagedResult<ConfigurationDriftRecordDto>>> GetConfigDrifts(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] Guid? deviceId = null,
        [FromQuery] bool? hasDrift = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetConfigurationDriftsPagedQuery(pageNumber, pageSize, deviceId, hasDrift),
            cancellationToken);
        return Ok(result);
    }

    [HttpPost("analyze/port-scans")]
    [HasPermission(Permissions.ThreatDetection.Analyze)]
    public async Task<ActionResult<IReadOnlyList<ThreatIndicatorDto>>> AnalyzePortScans(
        [FromQuery] int timeWindowMinutes = 60,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new AnalyzePortScansCommand(timeWindowMinutes), cancellationToken);
        return Ok(result);
    }

    [HttpPost("analyze/unauthorized-access")]
    [HasPermission(Permissions.ThreatDetection.Analyze)]
    public async Task<ActionResult<IReadOnlyList<ThreatIndicatorDto>>> AnalyzeUnauthorizedAccess(
        [FromQuery] int timeWindowMinutes = 60,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new AnalyzeUnauthorizedAccessCommand(timeWindowMinutes), cancellationToken);
        return Ok(result);
    }

    [HttpGet("summary")]
    [HasPermission(Permissions.ThreatDetection.View)]
    public async Task<ActionResult<ThreatSummaryDto>> GetSummary(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetThreatSummaryQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("rules")]
    [HasPermission(Permissions.ThreatDetection.View)]
    public async Task<ActionResult<PagedResult<ThreatDetectionRuleDto>>> GetRules(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetThreatRulesPagedQuery(pageNumber, pageSize), cancellationToken);
        return Ok(result);
    }

    [HttpPost("rules")]
    [HasPermission(Permissions.ThreatDetection.Manage)]
    public async Task<ActionResult<ThreatDetectionRuleDto>> CreateRule(
        [FromBody] CreateThreatRuleDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new CreateThreatRuleCommand(dto.RuleName, dto.ThreatType, dto.DefaultSeverity, dto.FailureThreshold, dto.TimeWindowMinutes, dto.IsEnabled, dto.Description),
            cancellationToken);
        return CreatedAtAction(nameof(GetRules), new { id = result.Id }, result);
    }
}