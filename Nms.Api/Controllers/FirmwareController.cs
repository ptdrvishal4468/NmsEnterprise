using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nms.Application.Common.Models;
using Nms.Application.Firmware.Commands.CreateFirmwareBaseline;
using Nms.Application.Firmware.Commands.CreateUpgradePlan;
using Nms.Application.Firmware.Commands.UpdateUpgradePlanStatus;
using Nms.Application.Firmware.Dtos;
using Nms.Application.Firmware.Queries.CompareFirmwareVersions;
using Nms.Application.Firmware.Queries.GetDeviceFirmwareInventoryPaged;
using Nms.Application.Firmware.Queries.GetFirmwareComplianceSummary;
using Nms.Application.Firmware.Queries.GetUpgradePlansPaged;
using Nms.Domain.Constants;
using Nms.Domain.Enums;
using Nms.Infrastructure.Security;

namespace Nms.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class FirmwareController : ControllerBase
{
    private readonly ISender _sender;

    public FirmwareController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Gets paged firmware inventory across tenant devices with optional compliance filtering.
    /// </summary>
    [HttpGet("inventory")]
    [HasPermission(Permissions.Firmware.View)]
    [ProducesResponseType(typeof(PagedResult<DeviceFirmwareInventoryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<DeviceFirmwareInventoryDto>>> GetInventory(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] FirmwareComplianceStatus? complianceStatus = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetDeviceFirmwareInventoryPagedQuery(pageNumber, pageSize, searchTerm, complianceStatus);
        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets high-level firmware compliance summary for the tenant.
    /// </summary>
    [HttpGet("compliance")]
    [HasPermission(Permissions.Firmware.View)]
    [ProducesResponseType(typeof(FirmwareComplianceSummaryDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<FirmwareComplianceSummaryDto>> GetComplianceSummary(CancellationToken cancellationToken = default)
    {
        var query = new GetFirmwareComplianceSummaryQuery();
        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Safely compares two software version strings.
    /// </summary>
    [HttpPost("compare")]
    [HasPermission(Permissions.Firmware.View)]
    [ProducesResponseType(typeof(FirmwareVersionComparisonDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<FirmwareVersionComparisonDto>> CompareVersions(
        [FromBody] CompareFirmwareVersionsQuery query,
        CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Creates or updates an approved target firmware baseline for a vendor/model combination.
    /// </summary>
    [HttpPost("baselines")]
    [HasPermission(Permissions.Firmware.ManageBaselines)]
    [ProducesResponseType(typeof(FirmwareBaselineDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<FirmwareBaselineDto>> CreateBaseline(
        [FromBody] CreateFirmwareBaselineCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Creates a firmware upgrade planning entry for a device.
    /// </summary>
    [HttpPost("upgrade-plans")]
    [HasPermission(Permissions.Firmware.ManagePlans)]
    [ProducesResponseType(typeof(FirmwareUpgradePlanDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<FirmwareUpgradePlanDto>> CreateUpgradePlan(
        [FromBody] CreateUpgradePlanCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetUpgradePlans), new { deviceId = result.DeviceId }, result);
    }

    /// <summary>
    /// Gets paged upgrade plans filtered by device or status.
    /// </summary>
    [HttpGet("upgrade-plans")]
    [HasPermission(Permissions.Firmware.View)]
    [ProducesResponseType(typeof(PagedResult<FirmwareUpgradePlanDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<FirmwareUpgradePlanDto>>> GetUpgradePlans(
        [FromQuery] Guid? deviceId = null,
        [FromQuery] UpgradePlanStatus? status = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetUpgradePlansPagedQuery(deviceId, status, pageNumber, pageSize);
        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Updates the status of an existing upgrade plan entry.
    /// </summary>
    [HttpPatch("upgrade-plans/{planId:guid}/status")]
    [HasPermission(Permissions.Firmware.ManagePlans)]
    [ProducesResponseType(typeof(FirmwareUpgradePlanDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<FirmwareUpgradePlanDto>> UpdateUpgradePlanStatus(
        [FromRoute] Guid planId,
        [FromBody] UpgradePlanStatus status,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateUpgradePlanStatusCommand(planId, status);
        var result = await _sender.Send(command, cancellationToken);
        return Ok(result);
    }
}