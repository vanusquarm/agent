using Transactions.API.Services;
using Finacle.Infrastructure.Auth;
using Finacle.Infrastructure.Auth.BasicJwt;
using Finacle.Infrastructure.Auth.OpenId;
using Serilog;
using Finacle.Core;
using Transactions.API.Filters;
using Transactions.API.Entities;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.Channels.json", optional: true, reloadOnChange: true);
var configuration = builder.Configuration;

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

builder.Host.UseSerilog(Log.Logger, true);

// Decrypt the client secret
var encryptedClientSecret = configuration["JwtSettings:ClientSecret"];
var decryptedClientSecret = EncryptionHelper.Decrypt(encryptedClientSecret);
configuration["JwtSettings:ClientSecret"] = decryptedClientSecret;

var finacleGateway = configuration["FinacleGateway:BaseUrl"] ?? throw new ArgumentNullException("FinacleGateway:BaseUrl");

// Add services to the container.
builder.Services.AddHttpClient<FinacleClient>(client =>
{
    client.BaseAddress = new Uri(finacleGateway);
    client.DefaultRequestHeaders.Add("ChannelId", configuration["FinacleGateway:ChannelId"]?.ToUpper());
});
builder.Services.AddScoped<TransferValidator>();
builder.Services.AddScoped<ValidateModelFilter>();

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddJwtMiddleware<ITransactionClient>(configuration);
builder.Services.AddOpenIdAuth(builder.Configuration, null);
builder.Services.Configure<ServiceMapOptions>(builder.Configuration.GetSection("Channels"));

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
