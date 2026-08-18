using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nms.Application.Common.Models;
using Nms.Application.Cybersecurity.Commands.CreateCompliancePolicy;
using Nms.Application.Cybersecurity.Commands.DeleteCompliancePolicy;
using Nms.Application.Cybersecurity.Commands.EvaluateAllDevicesCompliance;
using Nms.Application.Cybersecurity.Commands.EvaluateDeviceCompliance;
using Nms.Application.Cybersecurity.Commands.UpdateCompliancePolicy;
using Nms.Application.Cybersecurity.Dtos;
using Nms.Application.Cybersecurity.Queries.GetCompliancePoliciesPaged;
using Nms.Application.Cybersecurity.Queries.GetCompliancePolicyById;
using Nms.Application.Cybersecurity.Queries.GetCybersecurityPostureSummary;
using Nms.Application.Cybersecurity.Queries.GetDeviceComplianceScanById;
using Nms.Application.Cybersecurity.Queries.GetDeviceComplianceScansPaged;
using Nms.Application.Cybersecurity.Queries.GetDeviceLatestComplianceScan;
using Nms.Domain.Constants;
using Nms.Domain.Enums;
using Nms.Infrastructure.Security;

namespace Nms.Api.Controllers;

[ApiController]
[Route("api/v1/cybersecurity")]
[Authorize]
public class CybersecurityComplianceController : ControllerBase
{
    private readonly IMediator _mediator;

    public CybersecurityComplianceController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves a paged list of cybersecurity compliance policies.
    /// </summary>
    [HttpGet("policies")]
    [HasPermission(Permissions.Cybersecurity.View)]
    [ProducesResponseType(typeof(PagedResult<CompliancePolicyDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPolicies(
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? searchTerm = null,
        [FromQuery] ComplianceCategory? category = null,
        [FromQuery] ComplianceSeverity? severity = null,
        [FromQuery] bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetCompliancePoliciesPagedQuery(pageIndex, pageSize, searchTerm, category, severity, isActive);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a specific compliance policy by its unique identifier.
    /// </summary>
    [HttpGet("policies/{id:guid}")]
    [HasPermission(Permissions.Cybersecurity.View)]
    [ProducesResponseType(typeof(CompliancePolicyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPolicyById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCompliancePolicyByIdQuery(id), cancellationToken);
        if (result == null)
            return NotFound(new { message = $"Compliance policy with ID '{id}' was not found." });

        return Ok(result);
    }

    /// <summary>
    /// Creates a new cybersecurity compliance policy.
    /// </summary>
    [HttpPost("policies")]
    [HasPermission(Permissions.Cybersecurity.ManagePolicies)]
    [ProducesResponseType(typeof(CompliancePolicyDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePolicy([FromBody] CreateCompliancePolicyDto dto, CancellationToken cancellationToken)
    {
        var command = new CreateCompliancePolicyCommand(
            dto.Name,
            dto.Description,
            dto.Category,
            dto.CheckType,
            dto.Severity,
            dto.IsActive,
            dto.TargetVendor,
            dto.TargetDeviceType,
            dto.RuleConfigurationJson);

        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetPolicyById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Updates an existing cybersecurity compliance policy.
    /// </summary>
    [HttpPut("policies/{id:guid}")]
    [HasPermission(Permissions.Cybersecurity.ManagePolicies)]
    [ProducesResponseType(typeof(CompliancePolicyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePolicy(Guid id, [FromBody] UpdateCompliancePolicyDto dto, CancellationToken cancellationToken)
    {
        var command = new UpdateCompliancePolicyCommand(
            id,
            dto.Name,
            dto.Description,
            dto.Category,
            dto.CheckType,
            dto.Severity,
            dto.IsActive,
            dto.TargetVendor,
            dto.TargetDeviceType,
            dto.RuleConfigurationJson);

        var result = await _mediator.Send(command, cancellationToken);
        if (result == null)
            return NotFound(new { message = $"Compliance policy with ID '{id}' was not found." });

        return Ok(result);
    }

    /// <summary>
    /// Deletes a compliance policy by ID.
    /// </summary>
    [HttpDelete("policies/{id:guid}")]
    [HasPermission(Permissions.Cybersecurity.ManagePolicies)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePolicy(Guid id, CancellationToken cancellationToken)
    {
        var success = await _mediator.Send(new DeleteCompliancePolicyCommand(id), cancellationToken);
        if (!success)
            return NotFound(new { message = $"Compliance policy with ID '{id}' was not found." });

        return NoContent();
    }

    /// <summary>
    /// Triggers on-demand security compliance evaluation for a specific device.
    /// </summary>
    [HttpPost("evaluate/device")]
    [HasPermission(Permissions.Cybersecurity.Evaluate)]
    [ProducesResponseType(typeof(DeviceComplianceScanDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EvaluateDevice([FromBody] EvaluateDeviceComplianceDto dto, CancellationToken cancellationToken)
    {
        var command = new EvaluateDeviceComplianceCommand(dto.DeviceId, dto.EvaluationNotes, dto.SpecificPolicyIds);
        var result = await _mediator.Send(command, cancellationToken);
        if (result == null)
            return NotFound(new { message = $"Device with ID '{dto.DeviceId}' was not found." });

        return Ok(result);
    }

    /// <summary>
    /// Triggers bulk compliance evaluation across all managed devices in tenant scope.
    /// </summary>
    [HttpPost("evaluate/all")]
    [HasPermission(Permissions.Cybersecurity.Evaluate)]
    [ProducesResponseType(typeof(IReadOnlyList<DeviceComplianceScanDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> EvaluateAllDevices([FromBody] string? evaluationNotes, CancellationToken cancellationToken)
    {
        var command = new EvaluateAllDevicesComplianceCommand(evaluationNotes);
        var results = await _mediator.Send(command, cancellationToken);
        return Ok(results);
    }

    /// <summary>
    /// Retrieves a paged list of executed compliance scans.
    /// </summary>
    [HttpGet("scans")]
    [HasPermission(Permissions.Cybersecurity.View)]
    [ProducesResponseType(typeof(PagedResult<DeviceComplianceScanDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetScans(
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] Guid? deviceId = null,
        [FromQuery] ComplianceStatus? overallStatus = null,
        [FromQuery] DateTime? fromDateUtc = null,
        [FromQuery] DateTime? toDateUtc = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetDeviceComplianceScansPagedQuery(pageIndex, pageSize, deviceId, overallStatus, fromDateUtc, toDateUtc);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves full scan details and granular results for a specific scan ID.
    /// </summary>
    [HttpGet("scans/{scanId:guid}")]
    [HasPermission(Permissions.Cybersecurity.View)]
    [ProducesResponseType(typeof(DeviceComplianceScanDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetScanById(Guid scanId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetDeviceComplianceScanByIdQuery(scanId), cancellationToken);
        if (result == null)
            return NotFound(new { message = $"Compliance scan with ID '{scanId}' was not found." });

        return Ok(result);
    }

    /// <summary>
    /// Retrieves the most recent compliance scan and granular results for a target device.
    /// </summary>
    [HttpGet("devices/{deviceId:guid}/latest")]
    [HasPermission(Permissions.Cybersecurity.View)]
    [ProducesResponseType(typeof(DeviceComplianceScanDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDeviceLatestScan(Guid deviceId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetDeviceLatestComplianceScanQuery(deviceId), cancellationToken);
        if (result == null)
            return NotFound(new { message = $"No compliance scans found for device with ID '{deviceId}'." });

        return Ok(result);
    }

    /// <summary>
    /// Retrieves the aggregate cybersecurity compliance posture summary across all managed devices.
    /// </summary>
    [HttpGet("posture-summary")]
    [HasPermission(Permissions.Cybersecurity.View)]
    [ProducesResponseType(typeof(CybersecurityPostureSummaryDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPostureSummary(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCybersecurityPostureSummaryQuery(), cancellationToken);
        return Ok(result);
    }
}