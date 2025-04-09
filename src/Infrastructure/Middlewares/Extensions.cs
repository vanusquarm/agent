using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finacle.Infrastructure.Middlewares
{

    // Extension method for adding the middleware
    public static class Extensions
    {
        public static IApplicationBuilder UseRequestValidation(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequestValidationMiddleware>();
        }
    }
}
