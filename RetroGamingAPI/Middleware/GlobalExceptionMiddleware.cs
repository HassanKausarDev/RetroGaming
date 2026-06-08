using System.Text.Json;
using RetroGaming.Common.Exceptions;

namespace RetroGaming.API.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger)
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
            catch (AppException ex)
            {
                _logger.LogWarning("Business rule [{StatusCode}]: {Message}",
                    ex.StatusCode, ex.Message);
                await WriteErrorResponse(context, ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception on {Method} {Path}",
                    context.Request.Method, context.Request.Path);
                await WriteErrorResponse(context, 500,
                    ex.Message + " | " + ex.InnerException?.Message + " | " + ex.StackTrace);
            }
        }

        private static async Task WriteErrorResponse(
            HttpContext context, int statusCode, string message)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var body = JsonSerializer.Serialize(new
            {
                statusCode,
                message,
                timestamp = DateTime.UtcNow
            },
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(body);
        }
    }
}