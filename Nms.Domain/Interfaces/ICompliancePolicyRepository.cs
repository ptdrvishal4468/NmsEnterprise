using Nms.Domain.Entities;
using Nms.Domain.Enums;

namespace Nms.Domain.Interfaces;

/// <summary>
/// Repository contract for managing cybersecurity compliance policies.
/// </summary>
public interface ICompliancePolicyRepository : IGenericRepository<CompliancePolicy, Guid>
{
    Task<IReadOnlyList<CompliancePolicy>> GetActivePoliciesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CompliancePolicy>> GetActivePoliciesForDeviceTypeAsync(DeviceType deviceType, string? vendor = null, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<CompliancePolicy> Items, int TotalCount)> GetPagedAsync(
        int pageIndex,
        int pageSize,
        string? searchTerm,
        ComplianceCategory? category,
        ComplianceSeverity? severity,
        bool? isActive,
        CancellationToken cancellationToken = default);
}