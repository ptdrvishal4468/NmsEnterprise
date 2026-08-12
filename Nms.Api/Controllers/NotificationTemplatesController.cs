using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nms.Application.Common.Models;
using Nms.Application.Notifications.Commands.CreateNotificationTemplate;
using Nms.Application.Notifications.Commands.DeleteNotificationTemplate;
using Nms.Application.Notifications.Commands.SendTestNotification;
using Nms.Application.Notifications.Commands.UpdateNotificationTemplate;
using Nms.Application.Notifications.Dtos;
using Nms.Application.Notifications.Queries.GetNotificationLogsPaged;
using Nms.Application.Notifications.Queries.GetNotificationTemplatesPaged;
using Nms.Domain.Constants;
using Nms.Infrastructure.Security;

namespace Nms.Api.Controllers;

[ApiController]
[Route("api/v1/notification-templates")]
[Authorize]
public class NotificationTemplatesController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificationTemplatesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [HasPermission(Permissions.Notifications.View)]
    public async Task<ActionResult<PagedResult<NotificationTemplateDto>>> GetTemplates(
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetNotificationTemplatesPagedQuery(pageIndex, pageSize);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [HasPermission(Permissions.Notifications.ManageTemplates)]
    public async Task<ActionResult<NotificationTemplateDto>> CreateTemplate(
        [FromBody] CreateNotificationTemplateCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetTemplates), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [HasPermission(Permissions.Notifications.ManageTemplates)]
    public async Task<ActionResult<NotificationTemplateDto>> UpdateTemplate(
        Guid id,
        [FromBody] UpdateNotificationTemplateCommand command,
        CancellationToken cancellationToken = default)
    {
        if (id != command.Id)
        {
            return BadRequest("Mismatched template ID in route and body.");
        }

        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [HasPermission(Permissions.Notifications.ManageTemplates)]
    public async Task<IActionResult> DeleteTemplate(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(new DeleteNotificationTemplateCommand(id), cancellationToken);
        return NoContent();
    }

    [HttpPost("test")]
    [HasPermission(Permissions.Notifications.SendTest)]
    public async Task<ActionResult<bool>> SendTestNotification(
        [FromBody] SendTestNotificationCommand command,
        CancellationToken cancellationToken = default)
    {
        var success = await _mediator.Send(command, cancellationToken);
        return Ok(success);
    }

    [HttpGet("logs")]
    [HasPermission(Permissions.Notifications.View)]
    public async Task<ActionResult<PagedResult<NotificationLogDto>>> GetLogs(
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetNotificationLogsPagedQuery(pageIndex, pageSize);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}