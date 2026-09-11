using Microsoft.AspNetCore.Http;
using Nms.Application.Common.Interfaces;

namespace Nms.Infrastructure.Tenants;

public class HttpTenantResolver : ITenantResolver
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string TenantHeaderKey = "X-Tenant-Id";
    private const string TenantClaimType = "tenant_id";

    public HttpTenantResolver(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Task<Guid?> ResolveAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            return Task.FromResult<Guid?>(null);
        }

        // 1. Resolve from JWT Claim
        var tenantClaim = httpContext.User.FindFirst(TenantClaimType)?.Value;
        if (!string.IsNullOrWhiteSpace(tenantClaim) && Guid.TryParse(tenantClaim, out var claimTenantId))
        {
            return Task.FromResult<Guid?>(claimTenantId);
        }

        // 2. Fallback to HTTP Request Header (X-Tenant-Id)
        if (httpContext.Request.Headers.TryGetValue(TenantHeaderKey, out var headerValue))
        {
            var rawHeader = headerValue.ToString();
            if (!string.IsNullOrWhiteSpace(rawHeader) && Guid.TryParse(rawHeader, out var headerTenantId))
            {
                return Task.FromResult<Guid?>(headerTenantId);
            }
        }

        return Task.FromResult<Guid?>(null);
    }
}