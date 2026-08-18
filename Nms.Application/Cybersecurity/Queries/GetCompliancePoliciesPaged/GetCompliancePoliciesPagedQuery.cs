using MediatR;
using Nms.Application.Common.Models;
using Nms.Application.Cybersecurity.Dtos;
using Nms.Domain.Enums;

namespace Nms.Application.Cybersecurity.Queries.GetCompliancePoliciesPaged;

public record GetCompliancePoliciesPagedQuery(
    int PageIndex = 1,
    int PageSize = 20,
    string? SearchTerm = null,
    ComplianceCategory? Category = null,
    ComplianceSeverity? Severity = null,
    bool? IsActive = null) : IRequest<PagedResult<CompliancePolicyDto>>;