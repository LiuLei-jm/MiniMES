using MiniMES.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniMES.Infrastructure.Persistence.Seed;

public static class RoleSeeder
{
    public static List<Role> Seed()
    {
        return new List<Role>
        {
    new Role("Admin"),
new Role("User")
        };
    }
}
