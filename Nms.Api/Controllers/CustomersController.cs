using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nms.Application.Common.Models;
using Nms.Application.Customers.Commands.AddCustomerContact;
using Nms.Application.Customers.Commands.CreateCustomer;
using Nms.Application.Customers.Commands.DeleteCustomer;
using Nms.Application.Customers.Commands.DeleteCustomerContact;
using Nms.Application.Customers.Commands.UpdateCustomer;
using Nms.Application.Customers.Commands.UpdateCustomerContact;
using Nms.Application.Customers.Dtos;
using Nms.Application.Customers.Queries.GetCustomerById;
using Nms.Application.Customers.Queries.GetCustomerContacts;
using Nms.Application.Customers.Queries.GetCustomerHierarchy;
using Nms.Application.Customers.Queries.GetCustomersPaged;
using Nms.Domain.Constants;
using Nms.Domain.Enums;
using Nms.Infrastructure.Security;

namespace Nms.Api.Controllers;

[ApiController]
[Route("api/v1/customers")]
[Authorize]
public class CustomersController : ControllerBase
{
    private readonly ISender _sender;

    public CustomersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [HasPermission(Permissions.Customers.View)]
    [ProducesResponseType(typeof(PagedResult<CustomerDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<CustomerDto>>> GetCustomersPaged(
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? searchTerm = null,
        [FromQuery] CustomerStatus? status = null,
        [FromQuery] CustomerTier? tier = null,
        [FromQuery] Guid? parentCustomerId = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetCustomersPagedQuery(pageIndex, pageSize, searchTerm, status, tier, parentCustomerId);
        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [HasPermission(Permissions.Customers.View)]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerDto>> GetCustomerById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetCustomerByIdQuery(id);
        var result = await _sender.Send(query, cancellationToken);

        if (result == null)
        {
            return NotFound(new { message = $"Customer with ID '{id}' was not found." });
        }

        return Ok(result);
    }

    [HttpGet("{id:guid}/hierarchy")]
    [HasPermission(Permissions.Customers.View)]
    [ProducesResponseType(typeof(CustomerHierarchyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerHierarchyDto>> GetCustomerHierarchy(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetCustomerHierarchyQuery(id);
        var result = await _sender.Send(query, cancellationToken);

        if (result == null)
        {
            return NotFound(new { message = $"Customer with ID '{id}' was not found." });
        }

        return Ok(result);
    }

    [HttpPost]
    [HasPermission(Permissions.Customers.Create)]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CustomerDto>> CreateCustomer(
        [FromBody] CreateCustomerDto dto,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateCustomerCommand(dto);
        var result = await _sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetCustomerById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [HasPermission(Permissions.Customers.Update)]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerDto>> UpdateCustomer(
        Guid id,
        [FromBody] UpdateCustomerDto dto,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateCustomerCommand(id, dto);
        var result = await _sender.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [HasPermission(Permissions.Customers.Delete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCustomer(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteCustomerCommand(id);
        var success = await _sender.Send(command, cancellationToken);

        if (!success)
        {
            return NotFound(new { message = $"Customer with ID '{id}' was not found." });
        }

        return NoContent();
    }

    [HttpGet("{id:guid}/contacts")]
    [HasPermission(Permissions.Customers.View)]
    [ProducesResponseType(typeof(IReadOnlyList<CustomerContactDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<CustomerContactDto>>> GetCustomerContacts(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetCustomerContactsQuery(id);
        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{id:guid}/contacts")]
    [HasPermission(Permissions.Customers.Create)]
    [ProducesResponseType(typeof(CustomerContactDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerContactDto>> AddCustomerContact(
        Guid id,
        [FromBody] CustomerContactInputDto dto,
        CancellationToken cancellationToken = default)
    {
        var command = new AddCustomerContactCommand(id, dto);
        var result = await _sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetCustomerContacts), new { id = result.CustomerId }, result);
    }

    [HttpPut("{id:guid}/contacts/{contactId:guid}")]
    [HasPermission(Permissions.Customers.Update)]
    [ProducesResponseType(typeof(CustomerContactDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerContactDto>> UpdateCustomerContact(
        Guid id,
        Guid contactId,
        [FromBody] CustomerContactInputDto dto,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateCustomerContactCommand(id, contactId, dto);
        var result = await _sender.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}/contacts/{contactId:guid}")]
    [HasPermission(Permissions.Customers.Delete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCustomerContact(
        Guid id,
        Guid contactId,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteCustomerContactCommand(id, contactId);
        var success = await _sender.Send(command, cancellationToken);

        if (!success)
        {
            return NotFound(new { message = $"Contact with ID '{contactId}' was not found for customer '{id}'." });
        }

        return NoContent();
    }
}