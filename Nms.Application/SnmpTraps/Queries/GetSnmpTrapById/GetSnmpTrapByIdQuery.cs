using MediatR;
using Nms.Application.SnmpTraps.Dtos;

namespace Nms.Application.SnmpTraps.Queries.GetSnmpTrapById;

public sealed record GetSnmpTrapByIdQuery(Guid Id, Guid TenantId) : IRequest<SnmpTrapMessageDto?>;