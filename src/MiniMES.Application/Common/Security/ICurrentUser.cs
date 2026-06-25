using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniMES.Application.Common.Security;

public interface ICurrentUser
{
    Guid UserId { get; }
    string UserName { get; }
    List<string> Roles { get; }
    bool IsAuthenticated { get; }
}
