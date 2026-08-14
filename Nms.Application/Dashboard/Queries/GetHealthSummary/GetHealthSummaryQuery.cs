using MediatR;
using Nms.Application.Dashboard.Dtos;

namespace Nms.Application.Dashboard.Queries.GetHealthSummary;

public sealed record GetHealthSummaryQuery(int TopDegradedLimit = 5) : IRequest<HealthSummaryDto>;