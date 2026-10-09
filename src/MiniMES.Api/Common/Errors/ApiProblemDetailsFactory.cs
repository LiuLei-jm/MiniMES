using Microsoft.AspNetCore.Mvc;
using MiniMES.Application.Common.Models;
using ProblemDetails = Microsoft.AspNetCore.Mvc.ProblemDetails;

namespace MiniMES.Api.Common.Errors;

public static class ApiProblemDetailsFactory
{
    public static ValidationProblemDetails CreateValidationProblem(IEnumerable<FluentValidation.Results.ValidationFailure> failures) =>
        new(failures.GroupBy(x => x.PropertyName).ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage).ToArray()))
        { Title = "Validation Failed", Status = StatusCodes.Status400BadRequest, Type = "https://httpstatuses.com/400" };

    public static ProblemDetails CreateServerError(Exception ex) => new()
    { Title = "Internal Server Error", Detail = ex.Message, Status = StatusCodes.Status500InternalServerError, Type = "https://httpstatuses.com/500" };

    public static ProblemDetails CreateBusinessError(Error error, int status) => new()
    { Title = error.Message, Status = status, Type = $"https://httpstatuses.com/{status}", Extensions = { ["code"] = error.Code } };
}
