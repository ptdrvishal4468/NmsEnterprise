using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nms.Application.Common.Interfaces;
using Nms.Application.Common.Models;
using Nms.Application.SnmpTraps.Dtos;
using Nms.Application.SnmpTraps.Queries.GetSnmpTrapById;
using Nms.Application.SnmpTraps.Queries.GetSnmpTrapsPaged;
using Nms.Domain.Constants;
using Nms.Domain.Enums;
using Nms.Infrastructure.Security;

namespace Nms.Api.Controllers;

[ApiController]
[Route("api/v1/snmptraps")]
[Authorize]
public sealed class SnmpTrapsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ITenantContext _tenantContext;

    public SnmpTrapsController(IMediator mediator, ITenantContext tenantContext)
    {
        _mediator = mediator;
        _tenantContext = tenantContext;
    }

    [HttpGet]
    [HasPermission(Permissions.SnmpTrap.View)]
    [ProducesResponseType(typeof(PagedResult<SnmpTrapMessageDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSnmpTrapsPaged(
        [FromQuery] Guid? deviceId,
        [FromQuery] TrapSeverity? severity,
        [FromQuery] string? sourceIpAddress,
        [FromQuery] string? enterpriseOid,
        [FromQuery] DateTime? fromUtc,
        [FromQuery] DateTime? toUtc,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetSnmpTrapsPagedQuery(
            TenantId: _tenantContext.TenantId,
            DeviceId: deviceId,
            Severity: severity,
            SourceIpAddress: sourceIpAddress,
            EnterpriseOid: enterpriseOid,
            FromUtc: fromUtc,
            ToUtc: toUtc,
            PageNumber: pageNumber,
            PageSize: pageSize);

        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [HasPermission(Permissions.SnmpTrap.View)]
    [ProducesResponseType(typeof(SnmpTrapMessageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSnmpTrapById(Guid id, CancellationToken cancellationToken = default)
    {
        var query = new GetSnmpTrapByIdQuery(id, _tenantContext.TenantId);
        var result = await _mediator.Send(query, cancellationToken);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }
}