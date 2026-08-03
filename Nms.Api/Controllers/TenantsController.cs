using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nms.Application.Tenants.Commands.CreateTenant;
using Nms.Application.Tenants.Commands.UpdateTenant;
using Nms.Application.Tenants.Dtos;
using Nms.Application.Tenants.Queries.GetTenantById;
using Nms.Application.Tenants.Queries.GetTenantsPaged;
using Nms.Domain.Constants;
using Nms.Infrastructure.Security;

namespace Nms.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class TenantsController : ControllerBase
{
    private readonly ISender _mediator;

    public TenantsController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [HasPermission(Permissions.Tenants.View)]
    public async Task<ActionResult<IEnumerable<TenantDto>>> GetTenants(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetTenantsPagedQuery(pageNumber, pageSize);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [HasPermission(Permissions.Tenants.View)]
    public async Task<ActionResult<TenantDto>> GetTenantById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetTenantByIdQuery(id);
        var tenant = await _mediator.Send(query, cancellationToken);

        if (tenant == null)
        {
            return NotFound(new { Message = $"Tenant with ID '{id}' was not found." });
        }

        return Ok(tenant);
    }

    [HttpPost]
    [HasPermission(Permissions.Tenants.Create)]
    public async Task<ActionResult<TenantDto>> CreateTenant(
        [FromBody] CreateTenantCommand command,
        CancellationToken cancellationToken = default)
    {
        var tenant = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetTenantById), new { id = tenant.Id }, tenant);
    }

    [HttpPut("{id:guid}")]
    [HasPermission(Permissions.Tenants.Update)]
    public async Task<ActionResult<TenantDto>> UpdateTenant(
        Guid id,
        [FromBody] UpdateTenantCommand command,
        CancellationToken cancellationToken = default)
    {
        if (id != command.Id)
        {
            return BadRequest(new { Message = "Route ID and request body ID mismatch." });
        }

        var tenant = await _mediator.Send(command, cancellationToken);
        if (tenant == null)
        {
            return NotFound(new { Message = $"Tenant with ID '{id}' was not found." });
        }

        return Ok(tenant);
    }
}