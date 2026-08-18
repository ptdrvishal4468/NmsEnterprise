using MediatR;
using Nms.Application.Cybersecurity.Dtos;

namespace Nms.Application.Cybersecurity.Queries.GetCompliancePolicyById;

public record GetCompliancePolicyByIdQuery(Guid Id) : IRequest<CompliancePolicyDto?>;