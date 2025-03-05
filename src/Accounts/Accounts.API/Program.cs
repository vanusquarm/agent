// using Finacle.Infrastructure;
using Acccounts.API.Services;
using Finacle.Infrastructure.Auth;
using Finacle.Infrastructure.Auth.BasicJwt;
using Microsoft.Extensions.Configuration;
using Refit;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Decrypt the client secret
var encryptedClientSecret = builder.Configuration["JwtSettings:ClientSecret"];
var decryptedClientSecret = EncryptionHelper.Decrypt(encryptedClientSecret);
builder.Configuration["JwtSettings:ClientSecret"] = decryptedClientSecret;

// Add services to the container.
// builder.AddInfrastructure(enableSwagger: enableSwagger);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddJwtMiddleware<IAccountClient>(configuration);

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
