using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using Serilog;
using Statement.Web.Interfaces;
using Statement.Web.Middleware;
using Statement.Web.Services;
using System.Text.Json.Serialization;

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

// Register Services
builder.Services.AddScoped<ISSOService, SSOService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IMiddlewareClient, MiddlewareClient>();
builder.Services.AddScoped<IGeneratePdfService, GeneratePdfService>();
builder.Services.AddScoped<IStatementClient, StatementClient>();
builder.Services.AddScoped<IGenerateExcelService, GenerateExcelService>();
builder.Services.AddScoped<IBasisFallbackService, BasisFallbackService>();

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

// ============ ADD SWAGGER ============
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "GT Bank Statement API",
        Version = "v1",
        Description = "API for generating and emailing bank statements. Use API Key for authentication.",
        Contact = new OpenApiContact
        {
            Name = "GT Bank Ghana",
            Email = "support@gtbank.com"
        }
    });

    // Define API Key Authentication
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "API Key authentication. Enter your API key in the text input below.",
        Name = "X-API-Key",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });

    // Add security requirement
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                }
            },
            Array.Empty<string>()
        }
    });
});

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