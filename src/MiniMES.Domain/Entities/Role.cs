using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniMES.Domain.Entities;

public class Role
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    private Role() { }
    public Role(string name)
    {
        Id = Guid.NewGuid();
        Name = name;
    }
}
