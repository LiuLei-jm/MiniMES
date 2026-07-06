using MiniMES.Application.Authentication.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniMES.Infrastructure.Authentication;

public class LoginAttemptService : ILoginAttemptService
{
    private readonly Dictionary<string, (int Count, DateTime LastTry)> _cache = new();
    public bool IsBlocked(string username)
    {
        if (!_cache.ContainsKey(username)) return false;
        var (count, lastTry) = _cache[username];
        return count >= 10 && lastTry > DateTime.UtcNow.AddMinutes(-1);
    }

    public void RecordFailure(string username)
    {
        if (!_cache.ContainsKey(username))
        {
            _cache[username] = (1, DateTime.UtcNow);
            return;
        }
        var (count, _) = _cache[username];
        _cache[username] = (count + 1, DateTime.UtcNow);
    }

    public void Reset(string username)
    {
        _cache.Remove(username);
    }
}
