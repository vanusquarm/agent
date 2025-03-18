// using Finacle.Infrastructure;

using Finacle.Infrastructure.Auth.BasicJwt;
using Refit;
using Loans.API.Services;
using Finacle.Infrastructure.Auth;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Decrypt the client secret
var encryptedClientSecret = builder.Configuration["JwtSettings:ClientSecret"];
var decryptedClientSecret = EncryptionHelper.Decrypt(encryptedClientSecret);
builder.Configuration["JwtSettings:ClientSecret"] = decryptedClientSecret;

// Add services to the container.
// builder.AddInfrastructure(enableSwagger: enableSwagger);
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddJwtMiddleware<ILoanClient>(configuration);

builder.Services.AddRefitClient<ILoanClient>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(builder.Configuration["BaseUrl"]); // Replace with your API base URL
    });

var app = builder.Build();
// app.UseInfrastructure(builder.Environment, enableSwagger: enableSwagger);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
