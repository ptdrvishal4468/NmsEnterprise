using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nms.Application.Alerts.Commands.CreateAlertRule;
using Nms.Application.Alerts.Commands.DeleteAlertRule;
using Nms.Application.Alerts.Commands.UpdateAlertRule;
using Nms.Application.Alerts.Dtos;
using Nms.Application.Alerts.Queries.GetAlertRulesPaged;
using Nms.Application.Common.Models;
using Nms.Domain.Constants;
using Nms.Infrastructure.Security;

namespace Nms.Api.Controllers;

[ApiController]
[Route("api/v1/alert-rules")]
[Authorize]
public class AlertRulesController : ControllerBase
{
    private readonly ISender _mediator;

    public AlertRulesController(ISender mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Fetches a paged list of configured alert rules.
    /// </summary>
    [HttpGet]
    [HasPermission(Permissions.Alerts.View)]
    public async Task<ActionResult<PagedResult<AlertRuleDto>>> GetAlertRules(
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] Guid? deviceId = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAlertRulesPagedQuery(pageIndex, pageSize, deviceId);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Creates a new alert threshold rule.
    /// </summary>
    [HttpPost]
    [HasPermission(Permissions.Alerts.ManageRules)]
    public async Task<ActionResult<AlertRuleDto>> CreateAlertRule(
        [FromBody] CreateAlertRuleCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Updates an existing alert rule.
    /// </summary>
    [HttpPut("{id:guid}")]
    [HasPermission(Permissions.Alerts.ManageRules)]
    public async Task<ActionResult<AlertRuleDto>> UpdateAlertRule(
        Guid id,
        [FromBody] UpdateAlertRuleCommand command,
        CancellationToken cancellationToken = default)
    {
        if (id != command.Id)
        {
            return BadRequest("Mismatched alert rule ID.");
        }

        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Deletes an alert rule.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [HasPermission(Permissions.Alerts.ManageRules)]
    public async Task<ActionResult> DeleteAlertRule(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteAlertRuleCommand(id);
        var success = await _mediator.Send(command, cancellationToken);

        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }
}