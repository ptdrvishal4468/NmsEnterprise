using Microsoft.AspNetCore.Authorization;

namespace Nms.Infrastructure.Security;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class HasPermissionAttribute : AuthorizeAttribute
{
    public const string PolicyPrefix = "PERMISSION_";

    public HasPermissionAttribute(string permission)
    {
        Policy = $"{PolicyPrefix}{permission}";
    }
}