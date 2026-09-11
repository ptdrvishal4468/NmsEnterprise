using MediatR;

namespace Nms.Application.PollProfiles.Commands.DeletePollProfile;

public record DeletePollProfileCommand(Guid Id) : IRequest<bool>;