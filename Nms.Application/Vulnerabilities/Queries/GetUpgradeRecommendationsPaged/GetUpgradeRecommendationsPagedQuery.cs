using MediatR;
using Nms.Application.Common.Models;
using Nms.Application.Vulnerabilities.Dtos;
using Nms.Domain.Enums;

namespace Nms.Application.Vulnerabilities.Queries.GetUpgradeRecommendationsPaged;

public record GetUpgradeRecommendationsPagedQuery(
    int Page = 1,
    int PageSize = 20,
    Guid? DeviceId = null,
    RecommendationPriority? Priority = null,
    bool? IsApplied = null) : IRequest<PagedResult<FirmwareUpgradeRecommendationDto>>;