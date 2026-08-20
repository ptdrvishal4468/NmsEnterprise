using MediatR;
using Nms.Application.Dashboard.Dtos;

namespace Nms.Application.Dashboard.Queries.GetPerformanceDashboard;

public sealed record GetPerformanceDashboardQuery : IRequest<PerformanceDashboardDto>;