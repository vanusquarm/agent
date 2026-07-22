using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Statement.Web.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiKeyAuthAttribute : Attribute, IAsyncActionFilter
    {
        private const string ApiKeyHeaderName = "X-API-Key";

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Skip API key validation if the user is already authenticated via cookie
            if (context.HttpContext.User.Identity?.IsAuthenticated == true)
            {
                await next();
                return;
            }

            // Check if the request has the API key header
            if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
            {
                context.Result = new UnauthorizedObjectResult(new { message = "API Key is missing" });
                return;
            }

            // Get the configured API key from appsettings
            var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var apiKey = configuration["ApiKey"] ?? configuration["Authentication:ApiKey"];

            if (string.IsNullOrEmpty(apiKey))
            {
                context.Result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
                return;
            }

            // Validate the API key
            if (!string.Equals(extractedApiKey, apiKey, StringComparison.Ordinal))
            {
                context.Result = new UnauthorizedObjectResult(new { message = "Invalid API Key" });
                return;
            }

            await next();
        }
    }
}