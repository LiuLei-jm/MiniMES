using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using MiniMES.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniMES.Infrastructure.Persistence.Interceptors;

public class AppDbCommandInterceptor : DbCommandInterceptor
{
    private readonly ILogger<AppDbCommandInterceptor> _logger;
    private readonly ICurrentRequestContext _context;

    public AppDbCommandInterceptor(ILogger<AppDbCommandInterceptor> logger, ICurrentRequestContext context)
    {
        _logger = logger;
        _context = context;
    }

    public override async ValueTask<DbDataReader> ReaderExecutedAsync(
        DbCommand command,
        CommandExecutedEventData eventData,
        DbDataReader result,
        CancellationToken cancellationToken = default)
    {
        Log(command, eventData);
        return await base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
    }

    public override async ValueTask<int> NonQueryExecutedAsync(
        DbCommand command,
        CommandExecutedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        Log(command, eventData);
        return await base.NonQueryExecutedAsync(command, eventData, result, cancellationToken);
    }
    private void Log(DbCommand command, CommandExecutedEventData eventData)
    {
        var traceId = _context.TraceId;
        var elapsed = eventData.Duration.TotalMilliseconds;
        if (elapsed > 500)
        {
            _logger.LogWarning(
                "[{TraceId}] Infrastructure Slow SQL ({Elapsed}ms): {CommandText}",
                traceId,
                elapsed,
                command.CommandText
                );
        }
        else
        {
            _logger.LogInformation(
               "[{TraceId}] Infrastructure SQL ({Elapsed}ms: {CommandText}",
               traceId,
               elapsed,
               command.CommandText
                );
        }
    }
}
