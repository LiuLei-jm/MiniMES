using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using MiniMES.Application.Authentication.Constrants;
using MiniMES.Infrastructure.Authorization;
using Xunit;

namespace MiniMES.ArchitectureTests;

public sealed class PermissionAuthorizationTests
{
    [Fact]
    public async Task System_admin_permission_satisfies_any_permission_requirement()
    {
        var requirement = new PermissionRequirement("future:permission");
        var user = new ClaimsPrincipal(new ClaimsIdentity(
            [new Claim(CustomClaims.Permission, Permissions.System.Admin)],
            "test"));
        var context = new AuthorizationHandlerContext([requirement], user, null);
        var handler = new PermissionAuthorizationHandler();

        await handler.HandleAsync(context);

        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async Task User_without_required_or_system_admin_permission_is_denied()
    {
        var requirement = new PermissionRequirement(Permissions.Users.Create);
        var user = new ClaimsPrincipal(new ClaimsIdentity(
            [new Claim(CustomClaims.Permission, Permissions.Users.View)],
            "test"));
        var context = new AuthorizationHandlerContext([requirement], user, null);
        var handler = new PermissionAuthorizationHandler();

        await handler.HandleAsync(context);

        Assert.False(context.HasSucceeded);
    }
}
