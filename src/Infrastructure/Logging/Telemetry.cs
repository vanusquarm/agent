using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
//using Sentry.Extensions.Logging.Extensions.DependencyInjection;
using System;

namespace Finacle.Infrastructure.Logging
{
    public static class Telemetry
    {
        //public static IServiceCollection AddTelemetry(this IServiceCollection services, IConfiguration configuration)
        //{
        //    var telemetryProvider = configuration["Telemetry:Provider"]?.ToLowerInvariant();

        //    switch (telemetryProvider)
        //    {
        //        case "applicationinsights":
        //            services.AddApplicationInsightsTelemetry(configuration);
        //            break;
        //        case "sentry":
        //            services.AddSentryTelemetry(configuration);
        //            break;
        //        default:
        //            throw new InvalidOperationException("Telemetry provider is not configured correctly. Set 'Telemetry:Provider' to 'ApplicationInsights' or 'Sentry'.");
        //    }

        //    return services;
        //}

        //public static IServiceCollection AddApplicationInsightsTelemetry(this IServiceCollection services, IConfiguration configuration)
        //{
        //    services.AddApplicationInsightsTelemetry(options =>
        //    {
        //        options.ConnectionString = configuration["ApplicationInsights:ConnectionString"];
        //    });

        //    return services;
        //}

        //public static IServiceCollection AddSentryTelemetry(this IServiceCollection services, IConfiguration configuration)
        //{
        //    services.AddSentry(options =>
        //    {
        //        options.Dsn = configuration["Sentry:Dsn"];
        //    });
        //    return services;
        //}
    }
}