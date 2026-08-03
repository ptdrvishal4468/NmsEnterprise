using MediatR;
using Nms.Application.Tenants.Dtos;

namespace Nms.Application.Tenants.Commands.CreateTenant;

public record CreateTenantCommand(string Name) : IRequest<TenantDto>;