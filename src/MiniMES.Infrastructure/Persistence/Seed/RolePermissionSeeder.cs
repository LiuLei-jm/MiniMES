using MiniMES.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniMES.Infrastructure.Persistence.Seed;

public static class RolePermissionSeeder
{
    public static List<RolePermission> Seed(List<Role> roles, List<Permission> permissions)
    {
        var adminRole = roles.First(x => x.Name == "Admin");
        return permissions.Select(p => new RolePermission
        {
            RoleId = adminRole.Id,
            PermissionId = p.Id
        }).ToList();
    }
}
