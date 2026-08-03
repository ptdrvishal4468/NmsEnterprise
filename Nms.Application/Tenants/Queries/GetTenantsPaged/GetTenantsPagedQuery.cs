using MediatR;
using Nms.Application.Common.Models;
using Nms.Application.Tenants.Dtos;

namespace Nms.Application.Tenants.Queries.GetTenantsPaged;

public record GetTenantsPagedQuery(int PageNumber = 1, int PageSize = 10) : IRequest<PagedResult<TenantDto>>;