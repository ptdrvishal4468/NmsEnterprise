using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nms.Application.Topology.Commands.CreateTopologyLink;
using Nms.Application.Topology.Commands.DeleteTopologyLink;
using Nms.Application.Topology.Commands.DiscoverNeighbors;
using Nms.Application.Topology.Dtos;
using Nms.Application.Topology.Queries.GetDeviceTopology;
using Nms.Application.Topology.Queries.GetTopologyGraph;
using Nms.Domain.Constants;
using Nms.Domain.Enums;
using Nms.Infrastructure.Security;

namespace Nms.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class TopologyController : ControllerBase
{
    private readonly ISender _sender;

    public TopologyController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Gets the network topology graph (L2, L3, or both) for the tenant.
    /// </summary>
    [HttpGet]
    [HasPermission(Permissions.Topology.View)]
    [ProducesResponseType(typeof(TopologyGraphDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<TopologyGraphDto>> GetTopologyGraph(
        [FromQuery] TopologyLayerType? layerType = null,
        [FromQuery] TopologyLinkStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetTopologyGraphQuery(layerType, status);
        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets a localized topology subgraph centered on a specific device.
    /// </summary>
    [HttpGet("devices/{deviceId:guid}")]
    [HasPermission(Permissions.Topology.View)]
    [ProducesResponseType(typeof(TopologyGraphDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<TopologyGraphDto>> GetDeviceTopology(
        [FromRoute] Guid deviceId,
        [FromQuery] int depth = 2,
        [FromQuery] TopologyLayerType? layerType = null,
        [FromQuery] TopologyLinkStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetDeviceTopologyQuery(deviceId, depth, layerType, status);
        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Triggers an on-demand neighbor discovery scan across tenant network devices.
    /// </summary>
    [HttpPost("discover")]
    [HasPermission(Permissions.Topology.Discover)]
    [ProducesResponseType(typeof(DiscoverNeighborsResultDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<DiscoverNeighborsResultDto>> DiscoverNeighbors(
        [FromQuery] Guid? specificDeviceId = null,
        CancellationToken cancellationToken = default)
    {
        var command = new DiscoverNeighborsCommand(specificDeviceId);
        var result = await _sender.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Creates or updates a manual topology link between devices and optional interfaces.
    /// </summary>
    [HttpPost("links")]
    [HasPermission(Permissions.Topology.Manage)]
    [ProducesResponseType(typeof(TopologyLinkDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<TopologyLinkDto>> CreateTopologyLink(
        [FromBody] CreateTopologyLinkDto dto,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateTopologyLinkCommand(
            dto.SourceDeviceId,
            dto.TargetDeviceId,
            dto.LayerType,
            dto.Protocol,
            dto.SourceInterfaceId,
            dto.TargetInterfaceId,
            dto.SpeedBps,
            dto.MetadataJson);

        var result = await _sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetTopologyGraph), new { layerType = result.LayerType }, result);
    }

    /// <summary>
    /// Deletes an existing topology link.
    /// </summary>
    [HttpDelete("links/{id:guid}")]
    [HasPermission(Permissions.Topology.Manage)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTopologyLink(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteTopologyLinkCommand(id);
        var deleted = await _sender.Send(command, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}