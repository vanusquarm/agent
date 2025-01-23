using FluentValidation;
using Finacle.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Finacle.Infrastructure.Auth.BasicJwt;
using Microsoft.EntityFrameworkCore;
using Finacle.Infrastructure.Notifications;
using Finacle.Infrastructure.Swagger;

namespace Finacle.Infrastructure;

public static class Extensions
{
    public const string AllowAllOrigins = "AllowAll";
    public static void AddInfrastructure(this WebApplicationBuilder builder, Assembly? applicationAssembly = null, bool enableSwagger = true)
    {
        var config = builder.Configuration;

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: AllowAllOrigins,
                              builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        });
        //builder.Services.AddExceptionMiddleware(); 
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        // builder.ConfigureSerilog(appOptions.Name);
        builder.Services.AddRouting(options => options.LowercaseUrls = true);
        if (enableSwagger) builder.Services.AddSwaggerExtension(config);
    }

    public static void UseInfrastructure(this WebApplication app, IWebHostEnvironment env, bool enableSwagger = true)
    {
        //Preserve Order
        app.UseCors(AllowAllOrigins);
        //app.UseExceptionMiddleware();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        if (enableSwagger) app.UseSwaggerExtension(env);
    }
}
