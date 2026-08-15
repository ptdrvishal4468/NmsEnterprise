using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nms.Application.Common.Models;
using Nms.Application.Reporting.Commands.CreateScheduledReport;
using Nms.Application.Reporting.Commands.DeleteScheduledReport;
using Nms.Application.Reporting.Commands.ExecuteScheduledReport;
using Nms.Application.Reporting.Commands.UpdateScheduledReport;
using Nms.Application.Reporting.Dtos;
using Nms.Application.Reporting.Queries.GenerateAlertReport;
using Nms.Application.Reporting.Queries.GenerateDeviceReport;
using Nms.Application.Reporting.Queries.GenerateHealthReport;
using Nms.Application.Reporting.Queries.GenerateInventoryReport;
using Nms.Application.Reporting.Queries.GetScheduledReportExecutionLogsPaged;
using Nms.Application.Reporting.Queries.GetScheduledReportsPaged;
using Nms.Domain.Constants;
using Nms.Domain.Enums;
using Nms.Infrastructure.Security;

namespace Nms.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly ISender _sender;

    public ReportsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Generates an operational device report in JSON or CSV format.
    /// </summary>
    [HttpGet("devices")]
    [HasPermission(Permissions.Reports.Generate)]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GenerateDeviceReport(
        [FromQuery] DeviceType? deviceType = null,
        [FromQuery] DeviceStatus? status = null,
        [FromQuery] string? vendor = null,
        [FromQuery] string? site = null,
        [FromQuery] ReportFormat format = ReportFormat.Json,
        CancellationToken cancellationToken = default)
    {
        var query = new GenerateDeviceReportQuery(deviceType, status, vendor, site, format);
        var result = await _sender.Send(query, cancellationToken);
        return File(result.Data, result.ContentType, result.FileName);
    }

    /// <summary>
    /// Generates a device health and degradation report in JSON or CSV format.
    /// </summary>
    [HttpGet("health")]
    [HasPermission(Permissions.Reports.Generate)]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GenerateHealthReport(
        [FromQuery] DeviceStatus? status = null,
        [FromQuery] double? minHealthScore = null,
        [FromQuery] double? maxHealthScore = null,
        [FromQuery] DateTime? fromUtc = null,
        [FromQuery] DateTime? toUtc = null,
        [FromQuery] ReportFormat format = ReportFormat.Json,
        CancellationToken cancellationToken = default)
    {
        var query = new GenerateHealthReportQuery(status, minHealthScore, maxHealthScore, fromUtc, toUtc, format);
        var result = await _sender.Send(query, cancellationToken);
        return File(result.Data, result.ContentType, result.FileName);
    }

    /// <summary>
    /// Generates an inventory and capacity report in JSON or CSV format.
    /// </summary>
    [HttpGet("inventory")]
    [HasPermission(Permissions.Reports.Generate)]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GenerateInventoryReport(
        [FromQuery] string? vendor = null,
        [FromQuery] string? site = null,
        [FromQuery] ReportFormat format = ReportFormat.Json,
        CancellationToken cancellationToken = default)
    {
        var query = new GenerateInventoryReportQuery(vendor, site, format);
        var result = await _sender.Send(query, cancellationToken);
        return File(result.Data, result.ContentType, result.FileName);
    }

    /// <summary>
    /// Generates an alert incident lifecycle report in JSON or CSV format.
    /// </summary>
    [HttpGet("alerts")]
    [HasPermission(Permissions.Reports.Generate)]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GenerateAlertReport(
        [FromQuery] AlertSeverity? severity = null,
        [FromQuery] AlertState? state = null,
        [FromQuery] MetricType? metricType = null,
        [FromQuery] DateTime? fromUtc = null,
        [FromQuery] DateTime? toUtc = null,
        [FromQuery] ReportFormat format = ReportFormat.Json,
        CancellationToken cancellationToken = default)
    {
        var query = new GenerateAlertReportQuery(severity, state, metricType, fromUtc, toUtc, format);
        var result = await _sender.Send(query, cancellationToken);
        return File(result.Data, result.ContentType, result.FileName);
    }

    /// <summary>
    /// Retrieves a paginated list of configured scheduled reports.
    /// </summary>
    [HttpGet("schedules")]
    [HasPermission(Permissions.Reports.ManageSchedules)]
    [ProducesResponseType(typeof(PagedResult<ScheduledReportDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<ScheduledReportDto>>> GetScheduledReportsPaged(
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] ReportType? reportType = null,
        [FromQuery] bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetScheduledReportsPagedQuery(pageIndex, pageSize, reportType, isActive);
        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Creates a new scheduled report configuration.
    /// </summary>
    [HttpPost("schedules")]
    [HasPermission(Permissions.Reports.ManageSchedules)]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    public async Task<ActionResult<Guid>> CreateScheduledReport(
        [FromBody] CreateScheduledReportCommand command,
        CancellationToken cancellationToken = default)
    {
        var id = await _sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetScheduledReportsPaged), new { id }, id);
    }

    /// <summary>
    /// Updates an existing scheduled report configuration.
    /// </summary>
    [HttpPut("schedules/{id:guid}")]
    [HasPermission(Permissions.Reports.ManageSchedules)]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> UpdateScheduledReport(
        [FromRoute] Guid id,
        [FromBody] UpdateScheduledReportCommand command,
        CancellationToken cancellationToken = default)
    {
        if (id != command.Id)
        {
            return BadRequest("The route identifier does not match the command payload identifier.");
        }

        var result = await _sender.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Deletes a scheduled report configuration.
    /// </summary>
    [HttpDelete("schedules/{id:guid}")]
    [HasPermission(Permissions.Reports.ManageSchedules)]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> DeleteScheduledReport(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteScheduledReportCommand(id);
        var result = await _sender.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Triggers immediate on-demand execution of a scheduled report.
    /// </summary>
    [HttpPost("schedules/{id:guid}/execute")]
    [HasPermission(Permissions.Reports.ManageSchedules)]
    [ProducesResponseType(typeof(ScheduledReportExecutionLogDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ScheduledReportExecutionLogDto>> ExecuteScheduledReport(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new ExecuteScheduledReportCommand(id);
        var result = await _sender.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves paginated execution history logs for a specific scheduled report.
    /// </summary>
    [HttpGet("schedules/{id:guid}/logs")]
    [HasPermission(Permissions.Reports.ManageSchedules)]
    [ProducesResponseType(typeof(PagedResult<ScheduledReportExecutionLogDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<ScheduledReportExecutionLogDto>>> GetScheduledReportExecutionLogsPaged(
        [FromRoute] Guid id,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetScheduledReportExecutionLogsPagedQuery(id, pageIndex, pageSize);
        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }
}