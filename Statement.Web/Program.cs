using Microsoft.AspNetCore.Authentication.Cookies;
using Serilog;
using Statement.Web.Interfaces;
using Statement.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Configure Serilog FIRST (before building services)
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateBootstrapLogger();

// 2. Use Serilog
builder.Host.UseSerilog((ctx, lc) =>
{
    lc.ReadFrom.Configuration(ctx.Configuration)
      .Enrich.FromLogContext();
});

// Add services
builder.Services.AddControllersWithViews();

// === Your other services here ===
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

// Register your services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IMiddlewareClient, MiddlewareClient>();
builder.Services.AddScoped<IGeneratePdfService, GeneratePdfService>();
builder.Services.AddScoped<IStatementClient, StatementClient>();
//builder.Services.AddScoped<IGenerateExcelService, GenerateExcelService>();
builder.Services.AddScoped<IBasisFallbackService, BasisFallbackService>();

builder.Services.AddHttpClient<IMiddlewareClient, MiddlewareClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["FinacleSettings:BaseUrl"] ?? throw new InvalidOperationException("Finacle BaseUrl is not configured"));

    client.DefaultRequestHeaders.Add("Accept", "application/json");
});
builder.Services.AddHttpContextAccessor();
var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();