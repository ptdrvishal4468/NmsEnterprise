using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nms.Application.Interfaces.Commands.PollDeviceInterfaces;
using Nms.Application.Interfaces.Queries.GetDeviceInterfaces;
using Nms.Application.Interfaces.Queries.GetInterfaceHistory;
using Nms.Domain.Constants;
using Nms.Infrastructure.Security;

namespace Nms.Api.Controllers;

[ApiController]
[Route("api/v1")]
[Authorize]
public class InterfacesController : ControllerBase
{
    private readonly ISender _mediator;

    public InterfacesController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("devices/{deviceId:guid}/interfaces")]
    [HasPermission(Permissions.Interfaces.View)]
    public async Task<IActionResult> GetDeviceInterfaces(Guid deviceId, CancellationToken cancellationToken)
    {
        var query = new GetDeviceInterfacesQuery(deviceId);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("interfaces/{interfaceId:guid}/history")]
    [HasPermission(Permissions.Interfaces.View)]
    public async Task<IActionResult> GetInterfaceHistory(
        Guid interfaceId,
        [FromQuery] DateTime? startUtc,
        [FromQuery] DateTime? endUtc,
        CancellationToken cancellationToken)
    {
        var start = startUtc ?? DateTime.UtcNow.AddDays(-1);
        var end = endUtc ?? DateTime.UtcNow;

        var query = new GetInterfaceHistoryQuery(interfaceId, start, end);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost("devices/{deviceId:guid}/interfaces/poll")]
    [HasPermission(Permissions.Interfaces.Poll)]
    public async Task<IActionResult> PollDeviceInterfaces(Guid deviceId, CancellationToken cancellationToken)
    {
        var command = new PollDeviceInterfacesCommand(deviceId);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}