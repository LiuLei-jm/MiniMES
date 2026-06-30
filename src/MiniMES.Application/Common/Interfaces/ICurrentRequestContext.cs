using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniMES.Application.Common.Interfaces;

public interface ICurrentRequestContext
{
    string TraceId { get; }
    string? UserId { get; }
}
