using Nms.Application.Common.Interfaces;

namespace Nms.Infrastructure.Tenants;

public class TenantContext : ITenantContext
{
    private Guid _tenantId;

    public Guid TenantId => _tenantId;
    public bool IsResolved => _tenantId != Guid.Empty;

    public void SetTenantId(Guid tenantId)
    {
        _tenantId = tenantId;
    }
}