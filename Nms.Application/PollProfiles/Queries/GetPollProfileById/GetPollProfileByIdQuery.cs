using MediatR;
using Nms.Application.PollProfiles.Dtos;

namespace Nms.Application.PollProfiles.Queries.GetPollProfileById;

public record GetPollProfileByIdQuery(Guid Id) : IRequest<PollProfileDto?>;