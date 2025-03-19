using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.HttpLogging;
using Serilog;
using Finacle.Infrastructure.Swagger;
using Finacle.Infrastructure.Auth.OpenId;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
bool enableSwagger = false;

var policyNames = new List<string>
{
    "accounts:read",
    "accounts:write",
    "customers:read",
    "customers:write",
    "transactions:read",
    "transactions:write",
    "branches:read",
    "branches:write",
    "loans:read",
    "loans:write",
    "payments:read",
    "payments:write",
};

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

builder.Host.UseSerilog(Log.Logger, true);

builder.Services.AddOpenIdAuth(builder.Configuration, policyNames);

// builder.AddInfrastructure(enableSwagger: enableSwagger);
builder.Services.AddSwaggerExtension(configuration);

builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All;
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;
});

var app = builder.Build();

app.UseSwaggerExtension(app.Environment, configuration);
app.Use(async (context, next) =>
{
    context.Response.Headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains; preload";
    context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";

    await next();
});
app.MapGet("/", () => "Healthy");
app.UseRouting();
app.UseSerilogRequestLogging(); // Add Serilog request logging
app.MapReverseProxy(config =>
{
    config.Use(async (context, next) =>
    {
        string? token = await context.GetTokenAsync("access_token");
        context.Request.Headers["Authorization"] = $"Bearer {token}";

        await next().ConfigureAwait(false);
    });
});

app.Run();

