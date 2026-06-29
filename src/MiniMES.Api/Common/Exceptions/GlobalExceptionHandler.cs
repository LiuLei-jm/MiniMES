using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MiniMES.Api.Common.Errors;
using ProblemDetails = Microsoft.AspNetCore.Mvc.ProblemDetails;

namespace MiniMES.Api.Common.Exceptions;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Unhandled exception");

        httpContext.Response.ContentType = "application/problem+json";

        switch (exception)
        {
            case ValidationException validationException:
                var validationProblem = ApiProblemDetailsFactory
                    .CreateValidationProblem(validationException.Errors);
                httpContext.Response.StatusCode = validationProblem.Status!.Value;
                await httpContext.Response.WriteAsJsonAsync(validationProblem, cancellationToken);
                return true;
            default:
                var serverError = ApiProblemDetailsFactory.CreateServerError(exception);
                httpContext.Response.StatusCode = serverError.Status!.Value;
                await httpContext.Response.WriteAsJsonAsync(serverError, cancellationToken);
                return true;
        }
    }
}
