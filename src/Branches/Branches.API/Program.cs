using Branches.API.Services;
using Finacle.Infrastructure.Auth;
using Finacle.Infrastructure.Auth.BasicJwt;
using Finacle.Infrastructure.Auth.OpenId;
using Finacle.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Serilog;


var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

builder.Host.UseSerilog(Log.Logger, true);

// Decrypt the client secret
var encryptedClientSecret = configuration["JwtSettings:ClientSecret"];
var decryptedClientSecret = EncryptionHelper.Decrypt(encryptedClientSecret);
configuration["JwtSettings:ClientSecret"] = decryptedClientSecret;

// Register DbContext
builder.Services.AddDbContext<FinacleDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddJwtMiddleware<IBranchesClient>(configuration);
//builder.Services.AddOpenIdAuth(builder.Configuration, null); // Add Token Validation

var app = builder.Build();

app.UseSerilogRequestLogging(); // Add Serilog request logging

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use CORS policy
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
