using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniMES.Domain.Entities;

public class Permission
{
    public Guid Id { get; private set; }
    public string Code { get; private set; }
    public string Description { get; private set; }
    private Permission() { }
    public Permission(string code, string description)
    {
        Id = Guid.NewGuid();
        Code = code;
        Description = description;
    }
}
