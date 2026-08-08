using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nms.Application.Common.Models;
using Nms.Application.PollProfiles.Commands.CreatePollProfile;
using Nms.Application.PollProfiles.Commands.DeletePollProfile;
using Nms.Application.PollProfiles.Commands.UpdatePollProfile;
using Nms.Application.PollProfiles.Dtos;
using Nms.Application.PollProfiles.Queries.GetPollProfileById;
using Nms.Application.PollProfiles.Queries.GetPollProfilesPaged;
using Nms.Domain.Constants;
using Nms.Infrastructure.Security;

namespace Nms.Api.Controllers;

[ApiController]
[Route("api/v1/poll-profiles")]
[Authorize]
public class PollProfilesController : ControllerBase
{
    private readonly ISender _mediator;

    public PollProfilesController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [HasPermission(Permissions.Telemetry.Poll)]
    public async Task<ActionResult<Guid>> Create([FromBody] CreatePollProfileDto dto, CancellationToken cancellationToken)
    {
        var command = new CreatePollProfileCommand(dto);
        var id = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:guid}")]
    [HasPermission(Permissions.Telemetry.Poll)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePollProfileDto dto, CancellationToken cancellationToken)
    {
        var command = new UpdatePollProfileCommand(id, dto);
        var result = await _mediator.Send(command, cancellationToken);
        if (!result)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [HasPermission(Permissions.Telemetry.Poll)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeletePollProfileCommand(id);
        var result = await _mediator.Send(command, cancellationToken);
        if (!result)
            return NotFound();

        return NoContent();
    }

    [HttpGet("{id:guid}")]
    [HasPermission(Permissions.Telemetry.View)]
    public async Task<ActionResult<PollProfileDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetPollProfileByIdQuery(id);
        var profile = await _mediator.Send(query, cancellationToken);
        if (profile == null)
            return NotFound();

        return Ok(profile);
    }

    [HttpGet]
    [HasPermission(Permissions.Telemetry.View)]
    public async Task<ActionResult<PagedResult<PollProfileDto>>> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var query = new GetPollProfilesPagedQuery(pageNumber, pageSize);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}