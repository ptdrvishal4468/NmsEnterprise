using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nms.Application.Common.Interfaces;
using Nms.Application.Syslog.Dtos;
using Nms.Application.Syslog.Queries.GetSyslogById;
using Nms.Application.Syslog.Queries.GetSyslogsPaged;
using Nms.Domain.Constants;
using Nms.Domain.Enums;
using Nms.Infrastructure.Security;

namespace Nms.Api.Controllers;

[ApiController]
[Route("api/v1/syslog")]
public class SyslogController : ControllerBase
{
    private readonly ISender _mediator;
    private readonly ITenantContext _tenantContext;

    public SyslogController(ISender mediator, ITenantContext tenantContext)
    {
        _mediator = mediator;
        _tenantContext = tenantContext;
    }

    /// <summary>
    /// Retrieves a paged list of Syslog records matching search filters.
    /// </summary>
    [HttpGet]
    [HasPermission(Permissions.Syslog.View)]
    public async Task<IActionResult> GetSyslogsPaged(
        [FromQuery] Guid? deviceId,
        [FromQuery] SyslogSeverity? severity,
        [FromQuery] SyslogFacility? facility,
        [FromQuery] string? sourceIp,
        [FromQuery] string? searchKeyword,
        [FromQuery] DateTime? startDateUtc,
        [FromQuery] DateTime? endDateUtc,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantContext.TenantId;

        var query = new GetSyslogsPagedQuery(
            TenantId: tenantId,
            DeviceId: deviceId,
            Severity: severity,
            Facility: facility,
            SourceIp: sourceIp,
            SearchKeyword: searchKeyword,
            StartDateUtc: startDateUtc,
            EndDateUtc: endDateUtc,
            PageNumber: pageNumber,
            PageSize: pageSize);

        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves details of a specific Syslog entry by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [HasPermission(Permissions.Syslog.View)]
    public async Task<IActionResult> GetSyslogById(Guid id, CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantContext.TenantId;

        var query = new GetSyslogByIdQuery(id, tenantId);
        var result = await _mediator.Send(query, cancellationToken);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }
}