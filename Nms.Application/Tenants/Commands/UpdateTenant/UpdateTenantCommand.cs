using MediatR;
using Nms.Application.Tenants.Dtos;

namespace Nms.Application.Tenants.Commands.UpdateTenant;

public record UpdateTenantCommand(Guid Id, string Name, bool IsActive) : IRequest<TenantDto?>;