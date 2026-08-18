using MediatR;
using Nms.Application.Common.Models;
using Nms.Application.Vulnerabilities.Dtos;
using Nms.Domain.Enums;

namespace Nms.Application.Vulnerabilities.Queries.GetSecurityAdvisoriesPaged;

public record GetSecurityAdvisoriesPagedQuery(
    int Page = 1,
    int PageSize = 20,
    VulnerabilitySeverity? Severity = null,
    string? SearchTerm = null) : IRequest<PagedResult<SecurityAdvisoryDto>>;