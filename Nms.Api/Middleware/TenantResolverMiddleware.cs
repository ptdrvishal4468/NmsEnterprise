using Nms.Application.Common.Interfaces;
using Nms.Infrastructure.Tenants;

namespace Nms.Api.Middleware;

public class TenantResolverMiddleware
{
    private readonly RequestDelegate _next;

    public TenantResolverMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext httpContext,
        ITenantResolver tenantResolver,
        TenantContext tenantContext)
    {
        var resolvedTenantId = await tenantResolver.ResolveAsync();

        if (resolvedTenantId.HasValue && resolvedTenantId.Value != Guid.Empty)
        {
            tenantContext.SetTenantId(resolvedTenantId.Value);
        }

        await _next(httpContext);
    }
}