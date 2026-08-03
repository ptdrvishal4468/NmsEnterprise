using MediatR;
using Nms.Application.Tenants.Dtos;

namespace Nms.Application.Tenants.Queries.GetTenantById;

public record GetTenantByIdQuery(Guid Id) : IRequest<TenantDto?>;