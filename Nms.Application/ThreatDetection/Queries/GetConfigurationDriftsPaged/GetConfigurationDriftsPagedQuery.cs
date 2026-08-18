using MediatR;
using Nms.Application.Common.Models;
using Nms.Application.ThreatDetection.Dtos;

namespace Nms.Application.ThreatDetection.Queries.GetConfigurationDriftsPaged;

public record GetConfigurationDriftsPagedQuery(
    int PageNumber = 1,
    int PageSize = 20,
    Guid? DeviceId = null,
    bool? HasDrift = null) : IRequest<PagedResult<ConfigurationDriftRecordDto>>;