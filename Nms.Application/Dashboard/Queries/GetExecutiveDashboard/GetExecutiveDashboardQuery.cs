using MediatR;
using Nms.Application.Dashboard.Dtos;

namespace Nms.Application.Dashboard.Queries.GetExecutiveDashboard;

public sealed record GetExecutiveDashboardQuery : IRequest<ExecutiveDashboardDto>;