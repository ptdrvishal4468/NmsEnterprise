using MediatR;
using Nms.Application.PollProfiles.Dtos;

namespace Nms.Application.PollProfiles.Commands.UpdatePollProfile;

public record UpdatePollProfileCommand(Guid Id, UpdatePollProfileDto Dto) : IRequest<bool>;