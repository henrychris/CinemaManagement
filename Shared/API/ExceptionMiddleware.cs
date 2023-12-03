using System.Net;
using Shared.Responses;
using Shared.ServiceErrors;

namespace Shared.API;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
        }
        // can catch specific exceptions here.
        catch (Exception ex)
        {
            LogException(httpContext, ex);
            await HandleExceptionAsync(httpContext);
        }
    }

    private void LogException(HttpContext httpContext, Exception ex)
    {
        var http = httpContext.GetEndpoint()?.DisplayName?.Split(" => ")[0] ?? httpContext.Request.Path.ToString();
        var httpMethod = httpContext.Request.Method;
        var type = ex.GetType().Name;
        var error = ex.Message;
        var msg =
            $"""
             Something went wrong.
             =================================
             ENDPOINT: {http}
             METHOD: {httpMethod}
             TYPE: {type}
             REASON: {error}
             ---------------------------------
             {ex.StackTrace}
             """;
        logger.LogError("{@msg}", msg);
    }

    private static Task HandleExceptionAsync(HttpContext context)
    {
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var errors = new List<object>
        {
            new { GenericErrors.SystemError.Code, GenericErrors.SystemError.Description }
        };

        var response = new ApiErrorResponse<object>(errors, GenericErrors.SystemError.Description);
        return context.Response.WriteAsync(response.ToJsonString());
    }
}