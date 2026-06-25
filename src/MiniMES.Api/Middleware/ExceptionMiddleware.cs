using MiniMES.Application.Common.Results;
using System.Net;
using System.Text.Json;

namespace MiniMES.Api.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch(Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            var result = Result<string>.Failure(ex.Message);
            await context.Response.WriteAsync(JsonSerializer.Serialize(result));
        }
    }
}