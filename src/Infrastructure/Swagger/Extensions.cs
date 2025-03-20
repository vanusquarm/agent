using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;

namespace Finacle.Infrastructure.Swagger
{
    public static class Extensions
    {
        public static void UseSwaggerExtension(this IApplicationBuilder app, IWebHostEnvironment env, IConfiguration configuration)
        {
            if (env.IsProduction() || env.IsDevelopment())
            {
                app.UseCors("CorsPolicy");
                app.UseSwagger(c =>
                {
                    c.RouteTemplate = "swagger/{documentName}/swagger.json";
                    c.PreSerializeFilters.Add((swagger, httpReq) => swagger.Servers = new List<OpenApiServer> { new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}{httpReq.PathBase.Value}" } });
                });

                app.UseSwaggerUI(config =>
                {
                    config.SwaggerEndpoint("v1/swagger.json", "Version 1");
                    config.RoutePrefix = "swagger";
                    config.DocExpansion(DocExpansion.List);
                    config.DisplayRequestDuration();
                    config.DefaultModelsExpandDepth(-1);

                    // Add Swagger endpoints for each destination
                    var reverseProxyConfig = configuration.GetSection("ReverseProxy").Get<ReverseProxyOptions>();
                    foreach (var cluster in reverseProxyConfig.Clusters)
                    {
                        foreach (var destination in cluster.Value.Destinations)
                        {
                            if (destination.Value.Swaggers != null)
                            {
                                foreach (var swagger in destination.Value.Swaggers)
                                {
                                    foreach (var path in swagger.Paths)
                                    {
                                        config.SwaggerEndpoint($"{destination.Value.Address.Split(new []{ "/api" }, StringSplitOptions.None)[0]}{path}", $"{cluster.Key} - {swagger.PrefixPath}");
                                    }
                                }
                            }
                        }
                    }
                });
            }
        }

        public static void AddSwaggerExtension(this IServiceCollection services, IConfiguration configuration)
        {
            var swaggerOptions = configuration.GetSection("SwaggerOptions").Get<SwaggerOptions>();

            // Register CORS policy
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });


            _ = services.AddSwaggerGen(config =>
            {
                config.CustomSchemaIds(type => type.ToString());
                config.MapType<DateOnly>(() => new OpenApiSchema
                {
                    Type = "string",
                    Format = "date"
                });

                config.SwaggerDoc(
                    "v1",
                    new OpenApiInfo
                    {
                        Version = "v1",
                        Title = swaggerOptions.Title,
                        Description = swaggerOptions.Description,
                        Contact = new OpenApiContact
                        {
                            Name = swaggerOptions.Name,
                            Email = swaggerOptions.Email,
                        },
                    });

                config.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme {
                        Reference = new OpenApiReference {
                            Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                        }
                    },
                    Array.Empty<string>()
                }});

                config.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Input your Bearer token to access this API",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    BearerFormat = "JWT",
                });
            });
        }
    }
}
