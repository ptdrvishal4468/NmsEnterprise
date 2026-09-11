using MediatR;
using Nms.Application.Common.Models;
using Nms.Application.PollProfiles.Dtos;

namespace Nms.Application.PollProfiles.Queries.GetPollProfilesPaged;

public record GetPollProfilesPagedQuery(int PageNumber = 1, int PageSize = 10) : IRequest<PagedResult<PollProfileDto>>;