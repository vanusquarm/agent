using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finacle.Infrastructure.Auth.BasicJwt
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IJwtService _jwtService;

        public JwtMiddleware(RequestDelegate next, IJwtService jwtService)
        {
            _next = next;
            _jwtService = jwtService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Generate a new JWT token for every request
            var token = _jwtService.GenerateToken();

            // Add the JWT token to the Authorization header
            context.Request.Headers["Authorization"] = "Bearer " + token;

            // Call the next middleware in the pipeline
            await _next(context);
        }
    }
}
