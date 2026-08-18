using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nms.Application.Common.Models;
using Nms.Application.Vulnerabilities.Commands.AnalyzeAllDevicesVulnerabilities;
using Nms.Application.Vulnerabilities.Commands.AnalyzeDeviceVulnerabilities;
using Nms.Application.Vulnerabilities.Commands.CreateSecurityAdvisory;
using Nms.Application.Vulnerabilities.Commands.CreateVulnerability;
using Nms.Application.Vulnerabilities.Commands.GenerateUpgradeRecommendations;
using Nms.Application.Vulnerabilities.Commands.UpdateVulnerabilityMatchStatus;
using Nms.Application.Vulnerabilities.Dtos;
using Nms.Domain.Constants;
using Nms.Domain.Enums;
using Nms.Infrastructure.Security;

namespace Nms.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/vulnerabilities")]
public class VulnerabilitiesController : ControllerBase
{
    private readonly IMediator _mediator;

    public VulnerabilitiesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [HasPermission(Permissions.Vulnerabilities.View)]
    public async Task<ActionResult<PagedResult<VulnerabilityDto>>> GetVulnerabilities(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] VulnerabilitySeverity? severity = null,
        [FromQuery] string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        var query = new Application.Vulnerabilities.Queries.GetVulnerabilitiesPaged.GetVulnerabilitiesPagedQuery(page, pageSize, severity, searchTerm);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [HasPermission(Permissions.Vulnerabilities.View)]
    public async Task<ActionResult<VulnerabilityDto>> GetVulnerabilityById(Guid id, CancellationToken cancellationToken = default)
    {
        var query = new Application.Vulnerabilities.Queries.GetVulnerabilityById.GetVulnerabilityByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [HasPermission(Permissions.Vulnerabilities.Manage)]
    public async Task<ActionResult<VulnerabilityDto>> CreateVulnerability(
        [FromBody] CreateVulnerabilityDto dto,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateVulnerabilityCommand(
            dto.CveId,
            dto.Title,
            dto.Description,
            dto.Severity,
            dto.CvssScore,
            dto.AffectedVendor,
            dto.AffectedModel,
            dto.AffectedVersionMin,
            dto.AffectedVersionMax,
            dto.PatchedVersion,
            dto.PublishedAtUtc);

        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetVulnerabilityById), new { id = result.Id }, result);
    }

    [HttpGet("advisories")]
    [HasPermission(Permissions.Vulnerabilities.View)]
    public async Task<ActionResult<PagedResult<SecurityAdvisoryDto>>> GetSecurityAdvisories(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] VulnerabilitySeverity? severity = null,
        [FromQuery] string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        var query = new Application.Vulnerabilities.Queries.GetSecurityAdvisoriesPaged.GetSecurityAdvisoriesPagedQuery(page, pageSize, severity, searchTerm);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("advisories/{id:guid}")]
    [HasPermission(Permissions.Vulnerabilities.View)]
    public async Task<ActionResult<SecurityAdvisoryDto>> GetSecurityAdvisoryById(Guid id, CancellationToken cancellationToken = default)
    {
        var query = new Application.Vulnerabilities.Queries.GetSecurityAdvisoryById.GetSecurityAdvisoryByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost("advisories")]
    [HasPermission(Permissions.Vulnerabilities.Manage)]
    public async Task<ActionResult<SecurityAdvisoryDto>> CreateSecurityAdvisory(
        [FromBody] CreateSecurityAdvisoryDto dto,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateSecurityAdvisoryCommand(
            dto.AdvisoryId,
            dto.Vendor,
            dto.Title,
            dto.Summary,
            dto.Severity,
            dto.RemediationGuidance,
            dto.ReferenceUrl,
            dto.PublishedAtUtc);

        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetSecurityAdvisoryById), new { id = result.Id }, result);
    }

    [HttpPost("analyze/device/{deviceId:guid}")]
    [HasPermission(Permissions.Vulnerabilities.Analyze)]
    public async Task<ActionResult<IReadOnlyList<DeviceVulnerabilityMatchDto>>> AnalyzeDevice(Guid deviceId, CancellationToken cancellationToken = default)
    {
        var command = new AnalyzeDeviceVulnerabilitiesCommand(deviceId);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPost("analyze/all")]
    [HasPermission(Permissions.Vulnerabilities.Analyze)]
    public async Task<ActionResult<FirmwareRiskSummaryDto>> AnalyzeAllDevices(CancellationToken cancellationToken = default)
    {
        var command = new AnalyzeAllDevicesVulnerabilitiesCommand();
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpGet("matches")]
    [HasPermission(Permissions.Vulnerabilities.View)]
    public async Task<ActionResult<PagedResult<DeviceVulnerabilityMatchDto>>> GetDeviceVulnerabilityMatches(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] Guid? deviceId = null,
        [FromQuery] VulnerabilityStatus? status = null,
        [FromQuery] VulnerabilitySeverity? severity = null,
        CancellationToken cancellationToken = default)
    {
        var query = new Application.Vulnerabilities.Queries.GetDeviceVulnerabilitiesPaged.GetDeviceVulnerabilitiesPagedQuery(page, pageSize, deviceId, status, severity);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPut("matches/{matchId:guid}/status")]
    [HasPermission(Permissions.Vulnerabilities.Manage)]
    public async Task<IActionResult> UpdateMatchStatus(
        Guid matchId,
        [FromBody] UpdateVulnerabilityMatchStatusDto dto,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateVulnerabilityMatchStatusCommand(matchId, dto.Status, dto.Notes);
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPost("recommendations/generate")]
    [HasPermission(Permissions.Vulnerabilities.Recommendations)]
    public async Task<ActionResult<IReadOnlyList<FirmwareUpgradeRecommendationDto>>> GenerateUpgradeRecommendations(CancellationToken cancellationToken = default)
    {
        var command = new GenerateUpgradeRecommendationsCommand();
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpGet("recommendations")]
    [HasPermission(Permissions.Vulnerabilities.View)]
    public async Task<ActionResult<PagedResult<FirmwareUpgradeRecommendationDto>>> GetUpgradeRecommendations(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] Guid? deviceId = null,
        [FromQuery] RecommendationPriority? priority = null,
        [FromQuery] bool? isApplied = null,
        CancellationToken cancellationToken = default)
    {
        var query = new Application.Vulnerabilities.Queries.GetUpgradeRecommendationsPaged.GetUpgradeRecommendationsPagedQuery(page, pageSize, deviceId, priority, isApplied);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("risk-summary")]
    [HasPermission(Permissions.Vulnerabilities.View)]
    public async Task<ActionResult<FirmwareRiskSummaryDto>> GetFirmwareRiskSummary(CancellationToken cancellationToken = default)
    {
        var query = new Application.Vulnerabilities.Queries.GetFirmwareRiskSummary.GetFirmwareRiskSummaryQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}