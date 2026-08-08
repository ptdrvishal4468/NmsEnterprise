using MediatR;
using Nms.Application.PollProfiles.Dtos;

namespace Nms.Application.PollProfiles.Commands.CreatePollProfile;

public record CreatePollProfileCommand(CreatePollProfileDto Dto) : IRequest<Guid>;