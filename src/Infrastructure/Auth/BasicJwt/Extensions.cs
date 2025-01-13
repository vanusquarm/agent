using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finacle.Infrastructure.Auth.BasicJwt
{
    using Finacle.Infrastructure.Exceptions;
    using MassTransit.JobService;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using System.Threading.Tasks;

    public static class Extensions
    {
        public static IServiceCollection AddJwtMiddleware(this IServiceCollection services)
        {
            return services.AddScoped<IJwtService, JwtService>();
        }

        public static IApplicationBuilder UseJwtMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<JwtMiddleware>();
        }
    }

}
