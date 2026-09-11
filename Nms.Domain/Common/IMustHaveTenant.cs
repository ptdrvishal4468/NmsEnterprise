namespace Nms.Domain.Common;

/// <summary>
/// Contract enforced on all multi-tenant entities to guarantee logical data isolation.
/// </summary>
public interface IMustHaveTenant
{
    public Guid TenantId { get; set; }
}