using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.HttpLogging;
using Serilog;
using Serilog.Formatting.Compact;
using Serilog.Sinks.Elasticsearch;
using System.Reflection;
using Elastic.CommonSchema.Serilog; 

var builder = WebApplication.CreateBuilder(args);
bool enableSwagger = false;

var policyNames = new List<string>
{
    "identity:read",
    "identity:write",
};

// Configure Serilog
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var configuration = builder.Configuration;

var logFilePath = configuration["Serilog:WriteTo:2:Args:path"];

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithCorrelationId()
    .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day)
    .WriteTo.Console(new EcsTextFormatter())
    .Enrich.WithProperty("Environment", environment)
    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(configuration["ElasticConfiguration:Uri"]))
    {
        AutoRegisterTemplate = true,
        IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name.ToLower().Replace(".", "-")}-{environment?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}",
        CustomFormatter = new EcsTextFormatter()
    })
    .Enrich.WithProperty("Environment", environment)
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

builder.Host.UseSerilog(Log.Logger, true);

// builder.Services.AddOpenIdAuth(builder.Configuration, policyNames);

// builder.AddInfrastructure(enableSwagger: enableSwagger);
builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All;
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;
});

var app = builder.Build();
// app.UseInfrastructure(builder.Environment, enableSwagger: enableSwagger);
app.MapGet("/", () => "Healthy");
app.UseRouting();
//app.UseSerilogRequestLogging(); // Add Serilog request logging
app.MapReverseProxy(config =>
{
    config.Use(async (context, next) =>
    {
        //string? token = await context.GetTokenAsync("access_token");
        //context.Request.Headers["Authorization"] = $"Bearer {token}";

        await next().ConfigureAwait(false);
    });
});

app.Run();

