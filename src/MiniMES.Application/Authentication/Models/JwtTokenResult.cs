using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniMES.Application.Authentication.Models;

public sealed class JwtTokenResult
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
}
