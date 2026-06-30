using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniMES.Application.Common.Logging;

public sealed class LogEntry
{
    public string TraceId { get; set; } = default!;
    public string Layer { get; set; } = default!;
    public string Event { get; set; } = default!;
    public string RequestName { get; set; } = default!;
    public long? Elapsed { get; set; }
    public string? User { get; set; }
    public object? Data { get; set; }
}
