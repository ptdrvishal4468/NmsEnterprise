using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Nms.Infrastructure.Security;

public class PermissionAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
{
    public PermissionAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
        : base(options)
    {
    }

    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (policyName.StartsWith(HasPermissionAttribute.PolicyPrefix, StringComparison.OrdinalIgnoreCase))
        {
            var permission = policyName.Substring(HasPermissionAttribute.PolicyPrefix.Length);

            var policy = new AuthorizationPolicyBuilder();
            policy.AddRequirements(new PermissionRequirement(permission));
            return policy.Build();
        }

        return await base.GetPolicyAsync(policyName);
    }
}