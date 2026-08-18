using MediatR;
using Nms.Application.Vulnerabilities.Dtos;

namespace Nms.Application.Vulnerabilities.Queries.GetSecurityAdvisoryById;

public record GetSecurityAdvisoryByIdQuery(Guid Id) : IRequest<SecurityAdvisoryDto>;