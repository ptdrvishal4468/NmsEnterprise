using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nms.Application.Discovery.Commands.ImportDiscoveredDevice;
using Nms.Application.Discovery.Commands.StartDiscoveryScan;
using Nms.Application.Discovery.Dtos;
using Nms.Application.Discovery.Queries.GetDiscoveryJobById;
using Nms.Domain.Constants;
using Nms.Infrastructure.Security;

namespace Nms.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class DiscoveryController : ControllerBase
{
    private readonly ISender _mediator;

    public DiscoveryController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("scan")]
    [HasPermission(Permissions.Discovery.Scan)]
    public async Task<ActionResult<DiscoveryJobDto>> StartScan(
        [FromBody] StartDiscoveryScanDto dto,
        CancellationToken cancellationToken)
    {
        var command = new StartDiscoveryScanCommand(
            dto.Name,
            dto.IpRange,
            dto.SnmpCommunity,
            dto.SnmpPort);

        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetJobById), new { id = result.Id }, result);
    }

    [HttpGet("jobs/{id:guid}")]
    [HasPermission(Permissions.Discovery.View)]
    public async Task<ActionResult<DiscoveryJobDto>> GetJobById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetDiscoveryJobByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (result == null)
        {
            return NotFound(new { message = $"Discovery job '{id}' was not found." });
        }

        return Ok(result);
    }

    [HttpPost("jobs/{id:guid}/import")]
    [HasPermission(Permissions.Discovery.Import)]
    public async Task<IActionResult> ImportCandidate(
        Guid id,
        [FromBody] ImportDiscoveredDeviceDto dto,
        CancellationToken cancellationToken)
    {
        var command = new ImportDiscoveredDeviceCommand(id, dto.CandidateId, dto.DeviceName);
        var result = await _mediator.Send(command, cancellationToken);

        return Ok(result);
    }
}