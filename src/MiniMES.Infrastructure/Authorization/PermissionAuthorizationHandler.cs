using Microsoft.AspNetCore.Authorization;
using System;
using MiniMES.Application.Authentication.Constrants;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniMES.Infrastructure.Authorization;

public sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var permissions = context.User.Claims
            .Where(c => c.Type == CustomClaims.Permission)
            .Select(c => c.Value)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (permissions.Contains(requirement.Permission) ||
            permissions.Contains(Permissions.System.Admin))
        {
            context.Succeed(requirement);
        }
        return Task.CompletedTask;
    }
}
