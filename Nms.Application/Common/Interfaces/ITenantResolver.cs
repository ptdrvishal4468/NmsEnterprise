namespace Nms.Application.Common.Interfaces;

public interface ITenantResolver
{
    /// <summary>
    /// Attempts to resolve the Tenant ID from the current execution context.
    /// </summary>
    /// <returns>The resolved TenantId, or null if resolution fails.</returns>
    Task<Guid?> ResolveAsync();
}