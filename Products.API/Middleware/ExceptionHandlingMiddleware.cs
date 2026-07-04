using System.Net;
using System.Text.Json;
using FluentValidation;

namespace Products.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        // Thrown by ValidationBehavior in the MediatR pipeline — this is the only place
        // FluentValidation failures get turned into an HTTP response in the whole app.
        catch (ValidationException vex)
        {
            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            var errors = vex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            var problem = new
            {
                title = "One or more validation errors occurred.",
                status = context.Response.StatusCode,
                errors
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception processing {Method} {Path}", context.Request.Method, context.Request.Path);

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var problem = new
            {
                title = "An unexpected error occurred.",
                status = context.Response.StatusCode
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
        }
    }
}
