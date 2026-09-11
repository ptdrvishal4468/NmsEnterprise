namespace Nms.Application.Common.Interfaces;

public interface ITenantContext
{
    /// <summary>
    /// Gets the unique identifier of the current tenant.
    /// Will be empty (Guid.Empty) if no tenant context is resolved (e.g., system background jobs).
    /// </summary>
    Guid TenantId { get; }

    /// <summary>
    /// Indicates whether a valid tenant was successfully resolved for the current execution context.
    /// </summary>
    bool IsResolved { get; }
}