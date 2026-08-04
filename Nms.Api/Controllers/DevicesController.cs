using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nms.Application.Common.Models;
using Nms.Application.Devices.Commands.CreateDevice;
using Nms.Application.Devices.Commands.DeleteDevice;
using Nms.Application.Devices.Commands.UpdateDevice;
using Nms.Application.Devices.Dtos;
using Nms.Application.Devices.Queries.GetDeviceById;
using Nms.Application.Devices.Queries.GetDevicesPaged;
using Nms.Domain.Constants;
using Nms.Domain.Enums;
using Nms.Infrastructure.Security;

namespace Nms.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class DevicesController : ControllerBase
{
    private readonly ISender _mediator;

    public DevicesController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [HasPermission(Permissions.Devices.View)]
    public async Task<ActionResult<PagedResult<DeviceDto>>> GetDevices(
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] DeviceType? deviceType = null,
        [FromQuery] DeviceStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetDevicesPagedQuery(pageIndex, pageSize, searchTerm, deviceType, status);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [HasPermission(Permissions.Devices.View)]
    public async Task<ActionResult<DeviceDto>> GetDeviceById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetDeviceByIdQuery(id);
        var device = await _mediator.Send(query, cancellationToken);

        if (device == null)
        {
            return NotFound(new { Message = $"Device with ID '{id}' was not found." });
        }

        return Ok(device);
    }

    [HttpPost]
    [HasPermission(Permissions.Devices.Create)]
    public async Task<ActionResult<DeviceDto>> CreateDevice(
        [FromBody] CreateDeviceCommand command,
        CancellationToken cancellationToken = default)
    {
        var device = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetDeviceById), new { id = device.Id }, device);
    }

    [HttpPut("{id:guid}")]
    [HasPermission(Permissions.Devices.Update)]
    public async Task<ActionResult<DeviceDto>> UpdateDevice(
        Guid id,
        [FromBody] UpdateDeviceCommand command,
        CancellationToken cancellationToken = default)
    {
        if (id != command.Id)
        {
            return BadRequest(new { Message = "Route ID and request body ID mismatch." });
        }

        var device = await _mediator.Send(command, cancellationToken);
        return Ok(device);
    }

    [HttpDelete("{id:guid}")]
    [HasPermission(Permissions.Devices.Delete)]
    public async Task<ActionResult> DeleteDevice(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteDeviceCommand(id);
        var success = await _mediator.Send(command, cancellationToken);

        if (!success)
        {
            return NotFound(new { Message = $"Device with ID '{id}' was not found." });
        }

        return NoContent();
    }
}