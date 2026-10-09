using Microsoft.Extensions.Logging;

namespace MiniMES.Application.Common.Logging;

public static class LogExtensions
{
    public static void LogStructured(this ILogger logger, LogEntry entry)
    {
        logger.LogInformation("[{TraceId}] {Layer} {Event} {Request} {Elapsed}ms {@Data}", entry.TraceId, entry.Layer, entry.Event, entry.RequestName, entry.Elapsed ?? 0, Sanitize(entry.Data));
    }

    private static object? Sanitize(object? data)
    {
        if (data is null) return null;
        return data.GetType().GetProperties().ToDictionary(p => p.Name, p => IsSensitive(p.Name) ? "[REDACTED]" : p.GetValue(data));
    }

    private static bool IsSensitive(string name) => name.Equals("Password", StringComparison.OrdinalIgnoreCase) || name.Equals("RefreshToken", StringComparison.OrdinalIgnoreCase) || name.Equals("AccessToken", StringComparison.OrdinalIgnoreCase);
}
