using MediatR;
using Nms.Application.Common.Models;
using Nms.Application.ThreatDetection.Dtos;

namespace Nms.Application.ThreatDetection.Queries.GetThreatRulesPaged;

public record GetThreatRulesPagedQuery(int PageNumber = 1, int PageSize = 20) : IRequest<PagedResult<ThreatDetectionRuleDto>>;