using Application.Exceptions;
using System.Text.Json;

namespace API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next; _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleAsync(context, ex);
            }
        }

        private async Task HandleAsync(HttpContext ctx, Exception ex)
        {
            var traceId = ctx.TraceIdentifier;
            var (status, title, type, extensions) = ex switch
            {
                NotFoundException => (StatusCodes.Status404NotFound, "Not Found", "https://httpstatuses.io/404", (object?)null),
                ConflictException => (StatusCodes.Status409Conflict, "Conflict", "https://httpstatuses.io/409", (object?)null),
                ValidationException vex => (StatusCodes.Status400BadRequest, "Validation Error", "https://httpstatuses.io/400", new { errors = vex.Errors }),
                DomainException => (StatusCodes.Status400BadRequest, "Domain Error", "https://httpstatuses.io/400", (object?)null),
                _ => (StatusCodes.Status500InternalServerError, "Internal Server Error", "https://httpstatuses.io/500", (object?)null)
            };

            if (status >= 500) _logger.LogError(ex, "Unhandled exception. TraceId: {TraceId}", traceId);
            else _logger.LogWarning(ex, "Handled domain exception. TraceId: {TraceId}", traceId);

            var problem = new
            {
                type,
                title,
                status,
                detail = ex.Message,
                traceId
            };

            ctx.Response.ContentType = "application/problem+json";
            ctx.Response.StatusCode = status;

            var payload = extensions is null
                ? problem
                : Merge(problem, extensions);

            await ctx.Response.WriteAsync(JsonSerializer.Serialize(payload));
        }

        private static object Merge(object a, object b)
        {
            var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(JsonSerializer.Serialize(a))!;
            var ext = JsonSerializer.Deserialize<Dictionary<string, object>>(JsonSerializer.Serialize(b))!;

            foreach (var kv in ext) dict[kv.Key] = kv.Value;
            
            return dict;
        }
    }
}
