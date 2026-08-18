using Nms.Domain.Entities;
using Nms.Domain.Enums;

namespace Nms.Domain.Interfaces;

public interface ISecurityAdvisoryRepository : IGenericRepository<SecurityAdvisory, Guid>
{
    Task<SecurityAdvisory?> GetByAdvisoryIdAsync(Guid tenantId, string advisoryId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SecurityAdvisory>> GetByVendorAsync(Guid tenantId, string vendor, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<SecurityAdvisory> Items, int TotalCount)> GetPagedAsync(
        Guid tenantId,
        int page,
        int pageSize,
        VulnerabilitySeverity? severity = null,
        string? searchTerm = null,
        CancellationToken cancellationToken = default);
}