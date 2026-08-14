using MediatR;
using Nms.Application.Dashboard.Dtos;

namespace Nms.Application.Dashboard.Queries.GetTenantDashboard;

public sealed record GetTenantDashboardQuery(Guid? TenantId = null) : IRequest<TenantDashboardDto>;