using MediatR;
using Nms.Application.Common.Models;
using Nms.Application.Tenants.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Tenants.Queries.GetTenantsPaged;

public class GetTenantsPagedQueryHandler : IRequestHandler<GetTenantsPagedQuery, PagedResult<TenantDto>>
{
    private readonly ITenantRepository _tenantRepository;

    public GetTenantsPagedQueryHandler(ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<PagedResult<TenantDto>> Handle(GetTenantsPagedQuery request, CancellationToken cancellationToken)
    {
        var totalCount = await _tenantRepository.GetTotalCountAsync(cancellationToken);
        var tenants = await _tenantRepository.GetPagedAsync(request.PageNumber, request.PageSize, cancellationToken);

        var tenantDtos = tenants.Select(t => new TenantDto
        {
            Id = t.Id,
            Name = t.Name,
            IsActive = t.IsActive,
            CreatedAtUtc = t.CreatedAtUtc
        }).ToList();

        return new PagedResult<TenantDto>(tenantDtos, totalCount, request.PageNumber, request.PageSize);
    }
}