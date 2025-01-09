// using Finacle.Infrastructure;
// using Finacle.Infrastructure.Auth.OpenId;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);
bool enableSwagger = false;

var policyNames = new List<string>
{
    "identity:read",
    "identity:write",
};

// builder.Services.AddOpenIdAuth(builder.Configuration, policyNames);

// builder.AddInfrastructure(enableSwagger: enableSwagger);
builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
var app = builder.Build();
// app.UseInfrastructure(builder.Environment, enableSwagger: enableSwagger);
app.MapGet("/", () => "Hello From Gateway");
app.UseRouting();
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