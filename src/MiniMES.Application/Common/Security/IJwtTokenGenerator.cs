using MiniMES.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniMES.Application.Common.Security;

public interface IJwtTokenGenerator
{
    string Generate(User user, List<string> roles);
}
