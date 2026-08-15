using Nms.Domain.Entities;
using Nms.Domain.Enums;

namespace Nms.Domain.Interfaces;

/// <summary>
/// Specialized repository interface for querying and recording enterprise audit logs.
/// </summary>
public interface IAuditLogRepository : IGenericRepository<AuditLog, long>
{
    Task<(IReadOnlyList<AuditLog> Items, int TotalCount)> SearchAuditLogsAsync(
        Guid tenantId,
        DateTime? fromUtc,
        DateTime? toUtc,
        Guid? userId,
        string? action,
        AuditCategory? category,
        AuditStatus? status,
        string? entityName,
        string? entityId,
        string? searchTerm,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AuditLog>> GetConfigurationChangesAsync(
        Guid tenantId,
        DateTime? fromUtc,
        DateTime? toUtc,
        string? entityName,
        int maxCount,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<AuditLog> Items, int TotalCount)> GetUserActivityAsync(
        Guid tenantId,
        Guid? userId,
        DateTime? fromUtc,
        DateTime? toUtc,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default);
}