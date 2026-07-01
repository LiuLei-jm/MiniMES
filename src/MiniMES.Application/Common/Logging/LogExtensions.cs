using Microsoft.Extensions.Logging;

namespace MiniMES.Application.Common.Logging;

public static class LogExtensions
{
    public static void LogStructured(this ILogger logger, LogEntry entry)
    {
        logger.LogInformation(
            "[{TraceId}] {Layer} {Event} {Request} {Elapsed}ms {@Data}",
            entry.TraceId,
            entry.Layer,
            entry.Event,
            entry.RequestName,
            entry.Elapsed ?? 0,
            entry.Data
            );
    }
}
