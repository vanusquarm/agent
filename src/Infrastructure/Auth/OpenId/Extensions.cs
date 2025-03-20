using Finacle.Infrastructure.Exceptions;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Finacle.Infrastructure.Auth.OpenId;
public static class Extensions
{
    public static IServiceCollection AddOpenIdAuth(this IServiceCollection services, IConfiguration config, List<string> policyNames)
    {
        var allowedOrigins = config.GetSection("CorsOptions:AllowedOrigins").Get<string[]>();
        var authOptions = new OpenIdOptions();
        config.GetSection("JwtSettings").Bind(authOptions);

        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
            {
                builder.WithOrigins(allowedOrigins)
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowCredentials();
            });
        });

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.Authority = authOptions.Authority;
            options.Audience = authOptions.Audience;
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                RequireAudience = true,
                ValidateAudience = false,
            };
            //options.Events = new JwtBearerEvents
            //{
            //    OnChallenge = context =>
            //    {
            //        context.HandleResponse();
            //        if (!context.Response.HasStarted)
            //        {
            //            throw new UnauthorizedException(context.Error!, context.ErrorDescription!);
            //        }

            //        return Task.CompletedTask;
            //    },
            //    OnForbidden = _ => throw new ForbiddenException()
            //};
        });

        if (policyNames?.Count > 0)
        {
            services.AddAuthorization(options =>
            {
                foreach (string policyName in policyNames)
                {
                    options.AddPolicy(policyName, policy => policy.Requirements.Add(new HasScopeRequirement(policyName, authOptions.Authority!)));
                }
            });
        }

        services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
        return services;
    }
}
