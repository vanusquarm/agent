using Finacle.Infrastructure.Logging;
using Microsoft.AspNetCore.HttpLogging;
using Serilog;
using Finacle.Infrastructure.Swagger;
using Finacle.Infrastructure.Auth.OpenId;
using System.Collections.Concurrent;
using System;
using Microsoft.EntityFrameworkCore;
using Finacle.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.HttpOverrides;

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

//builder.Services.AddOpenIdAuth(builder.Configuration, policyNames);

// builder.AddInfrastructure(enableSwagger: enableSwagger);
//builder.Services.AddSwaggerExtension(configuration);

builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
builder.Services.AddOutputCache(options =>
{
    double.TryParse(configuration["CachingOptions:AbsoluteExpirationInMinutes"], out double ttl);
    options.AddPolicy("customPolicy", builder => builder.Expire(TimeSpan.FromMinutes(ttl)));
});

builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All;
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;
});

// Register DbContext
builder.Services.AddDbContext<FinacleDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

// Register CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear(); 
    options.KnownProxies.Clear();
});


var app = builder.Build();

//app.UseSwaggerExtension(app.Environment, configuration);
app.Use(async (context, next) =>
{
    context.Response.Headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains; preload";
    context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";

    context.Response.Headers["Access-Control-Allow-Origin"] = "*"; // Adjust as needed
    context.Response.Headers["Access-Control-Allow-Methods"] = "GET, POST, PUT, DELETE, OPTIONS";
    context.Response.Headers["Access-Control-Allow-Headers"] = "Content-Type, Authorization";


    await next();
});

app.UseForwardedHeaders();
app.UseRouting();
app.UseHttpLogging();
//app.UseAuthentication();
//app.UseAuthorization();
app.UseSerilogRequestLogging(); // Add Serilog request logging

app.UseOutputCache();
app.MapReverseProxy(config =>
{
    config.Use(async (context, next) =>
    {
        //string? token = await context.GetTokenAsync("access_token");
        //context.Request.Headers["Authorization"] = $"Bearer {token}";

        await next().ConfigureAwait(false);
    });
});

//app.UseMiddleware<RequestResponseLoggerMiddleware>();
app.UseRequestResponseLoggerMiddleware();
app.MapGet("/", () => "Healthy");

app.Run();