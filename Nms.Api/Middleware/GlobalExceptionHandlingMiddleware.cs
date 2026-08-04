using System.Net;
using System.Text.Json;
using FluentValidation;

namespace Nms.Api.Middleware;

public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
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
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed for request: {Path}", context.Request.Path);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            var errors = ex.Errors.Select(e => new
            {
                Field = e.PropertyName,
                Message = e.ErrorMessage
            });

            var responsePayload = new
            {
                Message = "Validation failed.",
                Errors = errors
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(responsePayload));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred processing request: {Path}", context.Request.Path);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var responsePayload = new
            {
                Message = "An unexpected server error occurred."
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(responsePayload));
        }
    }
}