using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Statement.Web.Middleware
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApiKeyMiddleware> _logger;
        private readonly IConfiguration _configuration;
        private const string ApiKeyHeaderName = "X-API-Key";

        public ApiKeyMiddleware(RequestDelegate next, ILogger<ApiKeyMiddleware> logger, IConfiguration configuration)
        {
            _next = next;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip API key validation for Swagger UI and other non-API paths
            if (context.Request.Path.StartsWithSegments("/swagger") ||
                context.Request.Path.StartsWithSegments("/Account") ||
                context.Request.Path.StartsWithSegments("/Home") ||
                context.Request.Path.StartsWithSegments("/Profile") ||
                !context.Request.Path.StartsWithSegments("/api"))
            {
                await _next(context);
                return;
            }

            // Skip if user is already authenticated via cookie
            if (context.User.Identity?.IsAuthenticated == true)
            {
                await _next(context);
                return;
            }

            if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(new { message = "API Key is missing" });
                return;
            }

            var apiKey = _configuration["ApiKey"] ?? _configuration["Authentication:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogError("API Key is not configured in appsettings");
                context.Response.StatusCode = 500;
                await context.Response.WriteAsJsonAsync(new { message = "Server configuration error" });
                return;
            }

            if (!string.Equals(extractedApiKey, apiKey, StringComparison.Ordinal))
            {
                _logger.LogWarning("Invalid API Key attempt");
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(new { message = "Invalid API Key" });
                return;
            }

            await _next(context);
        }
    }
}