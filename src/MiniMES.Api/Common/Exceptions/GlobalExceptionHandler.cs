using Microsoft.AspNetCore.Diagnostics;
using MiniMES.Api.Common.Errors;

namespace MiniMES.Api.Common.Exceptions;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "[{TraceId}] Unhandled exception", context.TraceIdentifier);
        context.Response.ContentType = "application/json";
        if (exception is BusinessException business)
        {
            var status = business.Error.Code switch
            {
                "Auth.InvalidCredential" or "Auth.InvalidRefreshToken" or "Token.InvalidRefreshToken" or "Token.RefreshExpired" or "Token.RefreshRevoked" => StatusCodes.Status401Unauthorized,
                "User.Locked" => StatusCodes.Status423Locked,
                "User.Disabled" => StatusCodes.Status403Forbidden,
                "Auth.TooManyAttempts" => StatusCodes.Status429TooManyRequests,
                "User.AlreadyExists" => StatusCodes.Status409Conflict,
                "User.CannotDisableSelf" => StatusCodes.Status409Conflict,
                "Role.AlreadyExists" or "Role.BuiltIn" or "Role.CannotModifyOwnRoles" => StatusCodes.Status409Conflict,
                "Role.NotFound" => StatusCodes.Status404NotFound,
                "Permission.AlreadyExists" or "Permission.Protected" => StatusCodes.Status409Conflict,
                "Permission.NotFound" => StatusCodes.Status404NotFound,
                "User.NotFound" => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status400BadRequest
            };
            context.Response.StatusCode = status;
            await context.Response.WriteAsJsonAsync(ApiProblemDetailsFactory.CreateBusinessError(business.Error, status), cancellationToken);
            return true;
        }
        if (exception is ValidationException validation)
        {
            var problem = ApiProblemDetailsFactory.CreateValidationProblem(validation.Errors);
            context.Response.StatusCode = problem.Status!.Value;
            await context.Response.WriteAsJsonAsync(problem, cancellationToken);
            return true;
        }
        var serverError = ApiProblemDetailsFactory.CreateServerError(exception);
        context.Response.StatusCode = serverError.Status!.Value;
        await context.Response.WriteAsJsonAsync(serverError, cancellationToken);
        return true;
    }
}
