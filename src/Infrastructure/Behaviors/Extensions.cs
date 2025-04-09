using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finacle.Infrastructure.Behaviors
{
    // Extension method for adding middleware
    public static class Extensions
    {
        public static IApplicationBuilder UseValidationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ValidationMiddleware>();
        }
    }

    public class ValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public ValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            if (endpoint == null)
            {
                await _next(context);
                return;
            }

            var modelState = context.Features.Get<Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary>();
            if (modelState != null && !modelState.IsValid)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsJsonAsync(new { Errors = modelState });
                return;
            }

            await _next(context);
        }
    }

}
