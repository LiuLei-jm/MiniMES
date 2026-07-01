using Microsoft.Extensions.Logging;
using MiniMES.Application.Common.Interfaces;
using MiniMES.Application.Common.Logging;

namespace MiniMES.Application.Common.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    private readonly ICurrentRequestContext _context;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger, ICurrentRequestContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var start = DateTime.UtcNow;
        _logger.LogStructured(new LogEntry
        {
            TraceId = _context.TraceId,
            Layer = "Application",
            Event = "Start",
            RequestName = typeof(TRequest).Name,
            Data = request
        });
        try
        {
            var response = await next();
            var elapsed = DateTime.UtcNow - start;
            _logger.LogStructured(new LogEntry
            {
                TraceId = _context.TraceId,
                Layer = "Application",
                Event = "End",
                RequestName = typeof(TRequest).Name,
                Elapsed = (long?)elapsed.TotalMilliseconds
            });
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[{TraceId}] Failed {Request}", _context.TraceId, typeof(TRequest).Name);
            throw;
        }
    }
}
