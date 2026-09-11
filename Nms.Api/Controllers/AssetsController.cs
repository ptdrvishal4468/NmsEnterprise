using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nms.Application.Assets.Commands.ChangeAssetLifecycle;
using Nms.Application.Assets.Commands.CreateAsset;
using Nms.Application.Assets.Commands.DeleteAsset;
using Nms.Application.Assets.Commands.UpdateAsset;
using Nms.Application.Assets.Dtos;
using Nms.Application.Assets.Queries.GetAssetById;
using Nms.Application.Assets.Queries.GetAssetsPaged;
using Nms.Application.Common.Models;
using Nms.Domain.Constants;
using Nms.Domain.Enums;
using Nms.Infrastructure.Security;

namespace Nms.Api.Controllers;

[ApiController]
[Route("api/v1/assets")]
[Authorize]
public class AssetsController : ControllerBase
{
    private readonly ISender _sender;

    public AssetsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [HasPermission(Permissions.Assets.View)]
    [ProducesResponseType(typeof(PagedResult<AssetDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<AssetDto>>> GetAssetsPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? searchTerm = null,
        [FromQuery] AssetLifecycleState? lifecycleState = null,
        [FromQuery] WarrantyStatus? warrantyStatus = null,
        [FromQuery] string? vendor = null,
        [FromQuery] string? department = null,
        [FromQuery] string? siteOrLocation = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAssetsPagedQuery(
            pageNumber,
            pageSize,
            searchTerm,
            lifecycleState,
            warrantyStatus,
            vendor,
            department,
            siteOrLocation);

        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [HasPermission(Permissions.Assets.View)]
    [ProducesResponseType(typeof(AssetDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AssetDto>> GetAssetById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAssetByIdQuery(id);
        var result = await _sender.Send(query, cancellationToken);

        if (result == null)
        {
            return NotFound(new { message = $"Asset with ID '{id}' was not found." });
        }

        return Ok(result);
    }

    [HttpPost]
    [HasPermission(Permissions.Assets.Create)]
    [ProducesResponseType(typeof(AssetDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AssetDto>> CreateAsset(
        [FromBody] CreateAssetDto dto,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateAssetCommand(
            AssetTag: dto.AssetTag,
            Name: dto.Name,
            LifecycleState: dto.LifecycleState,
            SerialNumber: dto.SerialNumber,
            Vendor: dto.Vendor,
            Model: dto.Model,
            Category: dto.Category,
            SiteOrLocation: dto.SiteOrLocation,
            RackIdentifier: dto.RackIdentifier,
            RackUnitPosition: dto.RackUnitPosition,
            Department: dto.Department,
            AssignedToUserId: dto.AssignedToUserId,
            DeviceId: dto.DeviceId,
            WarrantyProvider: dto.WarrantyProvider,
            WarrantyStartDateUtc: dto.WarrantyStartDateUtc,
            WarrantyEndDateUtc: dto.WarrantyEndDateUtc,
            WarrantyStatus: dto.WarrantyStatus,
            WarrantyContractNumber: dto.WarrantyContractNumber,
            PurchaseDateUtc: dto.PurchaseDateUtc,
            PurchaseOrderNumber: dto.PurchaseOrderNumber,
            PurchasePrice: dto.PurchasePrice,
            Currency: dto.Currency);

        var result = await _sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetAssetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [HasPermission(Permissions.Assets.Update)]
    [ProducesResponseType(typeof(AssetDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AssetDto>> UpdateAsset(
        Guid id,
        [FromBody] UpdateAssetDto dto,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateAssetCommand(
            Id: id,
            Name: dto.Name,
            SerialNumber: dto.SerialNumber,
            Vendor: dto.Vendor,
            Model: dto.Model,
            Category: dto.Category,
            SiteOrLocation: dto.SiteOrLocation,
            RackIdentifier: dto.RackIdentifier,
            RackUnitPosition: dto.RackUnitPosition,
            Department: dto.Department,
            AssignedToUserId: dto.AssignedToUserId,
            DeviceId: dto.DeviceId,
            WarrantyProvider: dto.WarrantyProvider,
            WarrantyStartDateUtc: dto.WarrantyStartDateUtc,
            WarrantyEndDateUtc: dto.WarrantyEndDateUtc,
            WarrantyStatus: dto.WarrantyStatus,
            WarrantyContractNumber: dto.WarrantyContractNumber,
            PurchaseDateUtc: dto.PurchaseDateUtc,
            PurchaseOrderNumber: dto.PurchaseOrderNumber,
            PurchasePrice: dto.PurchasePrice,
            Currency: dto.Currency);

        var result = await _sender.Send(command, cancellationToken);

        if (result == null)
        {
            return NotFound(new { message = $"Asset with ID '{id}' was not found." });
        }

        return Ok(result);
    }

    [HttpPatch("{id:guid}/lifecycle")]
    [HasPermission(Permissions.Assets.ManageLifecycle)]
    [ProducesResponseType(typeof(AssetDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AssetDto>> ChangeLifecycle(
        Guid id,
        [FromBody] ChangeAssetLifecycleDto dto,
        CancellationToken cancellationToken = default)
    {
        var command = new ChangeAssetLifecycleCommand(id, dto.LifecycleState, dto.Notes);
        var result = await _sender.Send(command, cancellationToken);

        if (result == null)
        {
            return NotFound(new { message = $"Asset with ID '{id}' was not found." });
        }

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [HasPermission(Permissions.Assets.Delete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsset(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteAssetCommand(id);
        var success = await _sender.Send(command, cancellationToken);

        if (!success)
        {
            return NotFound(new { message = $"Asset with ID '{id}' was not found." });
        }

        return NoContent();
    }
}