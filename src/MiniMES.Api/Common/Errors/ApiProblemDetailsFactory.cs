using Microsoft.AspNetCore.Mvc;
using ProblemDetails = Microsoft.AspNetCore.Mvc.ProblemDetails;

namespace MiniMES.Api.Common.Errors;

public static class ApiProblemDetailsFactory
{
    public static ValidationProblemDetails CreateValidationProblem(
        IEnumerable<FluentValidation.Results.ValidationFailure> failures)
    {
        var errors = failures
            .GroupBy(x => x.PropertyName)
           .ToDictionary(
            g => g.Key,
            g => g.Select(x => x.ErrorMessage).ToArray());
        return new ValidationProblemDetails(errors)
        {
            Title = "Validation Failed",
            Status = StatusCodes.Status400BadRequest,
            Type = "https://httpstatuses.com/400"
        };
    }
    public static ProblemDetails CreateServerError(Exception ex)
    {
        return new ProblemDetails
        {
            Title = "Internal Server Error",
            Detail = ex.Message,
            Status = StatusCodes.Status500InternalServerError,
            Type = "https://httpstatus.com/500"
        };
    }
}
