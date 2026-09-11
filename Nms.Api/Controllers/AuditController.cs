using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nms.Application.Auditing.Commands.RecordAuditLog;
using Nms.Application.Auditing.Dtos;
using Nms.Application.Auditing.Queries.GenerateComplianceReport;
using Nms.Application.Auditing.Queries.GetAuditLogById;
using Nms.Application.Auditing.Queries.GetConfigurationChanges;
using Nms.Application.Auditing.Queries.GetUserActivity;
using Nms.Application.Auditing.Queries.SearchAuditLogs;
using Nms.Application.Common.Models;
using Nms.Application.Reporting.Dtos;
using Nms.Domain.Constants;
using Nms.Domain.Enums;
using Nms.Infrastructure.Security;

namespace Nms.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class AuditController : ControllerBase
{
    private readonly ISender _sender;

    public AuditController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Searches and filters enterprise audit logs with database-side pagination.
    /// </summary>
    [HttpGet("search")]
    [HasPermission(Permissions.Audit.Search)]
    [ProducesResponseType(typeof(PagedResult<AuditLogDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search(
        [FromQuery] DateTime? fromUtc,
        [FromQuery] DateTime? toUtc,
        [FromQuery] Guid? userId,
        [FromQuery] string? action,
        [FromQuery] AuditCategory? category,
        [FromQuery] AuditStatus? status,
        [FromQuery] string? entityName,
        [FromQuery] string? entityId,
        [FromQuery] string? searchTerm,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new SearchAuditLogsQuery(
            FromUtc: fromUtc,
            ToUtc: toUtc,
            UserId: userId,
            Action: action,
            Category: category,
            Status: status,
            EntityName: entityName,
            EntityId: entityId,
            SearchTerm: searchTerm,
            PageIndex: pageIndex,
            PageSize: pageSize);

        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a single audit log entry by its unique identifier.
    /// </summary>
    [HttpGet("{id:long}")]
    [HasPermission(Permissions.Audit.View)]
    [ProducesResponseType(typeof(AuditLogDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetAuditLogByIdQuery(id), cancellationToken);
        if (result == null)
            return NotFound(new { message = $"Audit log with ID {id} was not found." });

        return Ok(result);
    }

    /// <summary>
    /// Retrieves user activity history and operational summaries.
    /// </summary>
    [HttpGet("user-activity")]
    [HasPermission(Permissions.Audit.View)]
    [ProducesResponseType(typeof(PagedResult<AuditLogDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserActivity(
        [FromQuery] Guid? userId,
        [FromQuery] DateTime? fromUtc,
        [FromQuery] DateTime? toUtc,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetUserActivityQuery(
            UserId: userId,
            FromUtc: fromUtc,
            ToUtc: toUtc,
            PageIndex: pageIndex,
            PageSize: pageSize);

        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves recent configuration modifications and value diffs.
    /// </summary>
    [HttpGet("configuration-changes")]
    [HasPermission(Permissions.Audit.View)]
    [ProducesResponseType(typeof(IReadOnlyList<ConfigurationChangeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetConfigurationChanges(
        [FromQuery] DateTime? fromUtc,
        [FromQuery] DateTime? toUtc,
        [FromQuery] string? entityName,
        [FromQuery] int maxCount = 50,
        CancellationToken cancellationToken = default)
    {
        var query = new GetConfigurationChangesQuery(
            FromUtc: fromUtc,
            ToUtc: toUtc,
            EntityName: entityName,
            MaxCount: maxCount);

        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Generates structured compliance summaries and exports in JSON or CSV format.
    /// </summary>
    [HttpGet("compliance-report")]
    [HasPermission(Permissions.Audit.ComplianceReport)]
    [ProducesResponseType(typeof(ComplianceReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GenerateComplianceReport(
        [FromQuery] DateTime? fromUtc,
        [FromQuery] DateTime? toUtc,
        [FromQuery] ReportFormat format = ReportFormat.Json,
        CancellationToken cancellationToken = default)
    {
        var query = new GenerateComplianceReportQuery(fromUtc, toUtc, format);
        var result = await _sender.Send(query, cancellationToken);

        if (result is ReportExportResultDto exportDto)
        {
            return File(exportDto.Data, exportDto.ContentType, exportDto.FileName);
        }

        return Ok(result);
    }

    /// <summary>
    /// Records a custom audit event into the enterprise audit trail.
    /// </summary>
    [HttpPost("record")]
    [HasPermission(Permissions.Audit.Search)]
    [ProducesResponseType(typeof(long), StatusCodes.Status201Created)]
    public async Task<IActionResult> Record(
        [FromBody] RecordAuditLogCommand command,
        CancellationToken cancellationToken)
    {
        var id = await _sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }
}