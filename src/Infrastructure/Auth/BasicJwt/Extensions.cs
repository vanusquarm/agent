using Refit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Finacle.Infrastructure.Auth.OpenId;

namespace Finacle.Infrastructure.Auth.BasicJwt
{

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

        public static IServiceCollection AddJwtMiddleware<T>(this IServiceCollection services, IConfiguration configuration) where T : class
        {
            services.Configure<JwtOptions>(configuration.GetSection("JwtSettings"));

            services.AddTransient<AuthHttpClientHandler>();
            services.AddSingleton<IJwtService, JwtService>();

            var baseUrl = configuration["BaseUrl"];
            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new ArgumentNullException("BaseUrl configuration is missing.");
            }

            // Register the Refit client with HttpClientFactory
            services.AddRefitClient<T>()
                .ConfigureHttpClient(c =>
                {
                    // Setting the BaseAddress for Finacle ApiHub
                    c.BaseAddress = new Uri(baseUrl);
                })
                .ConfigurePrimaryHttpMessageHandler(() =>
                {
                    return new HttpClientHandler
                    {
                        // Disabling SSL validation, but ensure this is safe and intentional
                        ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
                    };
                })
                .AddHttpMessageHandler<AuthHttpClientHandler>(); // Add the custom handler for adding the token to requests

            return services;
        }

    }

}
