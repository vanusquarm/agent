using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using Serilog;
using Statement.Web.Filters;
using Statement.Web.Interfaces;
using Statement.Web.Middleware;
using Statement.Web.Services;
using System.Text.Json.Serialization;
using Statement.Web.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateBootstrapLogger();

builder.Host.UseSerilog((ctx, lc) =>
{
    lc.ReadFrom.Configuration(ctx.Configuration)
      .Enrich.FromLogContext();
});

builder.Services.AddControllersWithViews();

// Add Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

// Add Authentication (Cookie for Web)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(30);
        options.SlidingExpiration = true;
        options.Cookie.Name = "GTBankAuth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.SameAsRequest;
    });

builder.AddApplicationServices();

// Configure HttpClient for MiddlewareClient
builder.Services.AddHttpClient<IMiddlewareClient, MiddlewareClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["FinacleSettings:BaseUrl"] ??
        throw new InvalidOperationException("Finacle BaseUrl is not configured"));
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// Add Controllers with JSON options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Add HttpContextAccessor
builder.Services.AddHttpContextAccessor();

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "GT Bank Statement API V1");
        c.RoutePrefix = "swagger";
        c.DocumentTitle = "GT Bank Statement API Documentation";
        c.DisplayRequestDuration();
        c.EnableTryItOutByDefault();
        c.EnableFilter();
    });
}

app.UseSerilogRequestLogging();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();

// Register API Key Middleware
app.UseMiddleware<ApiKeyMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();

app.Run();