using MediatR;
using Nms.Application.Dashboard.Dtos;

namespace Nms.Application.Dashboard.Queries.GetAlertSummary;

public sealed record GetAlertSummaryQuery(int TopRulesLimit = 5) : IRequest<AlertSummaryDto>;