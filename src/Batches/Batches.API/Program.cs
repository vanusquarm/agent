// using Finacle.Infrastructure;
using Finacle.Infrastructure.Auth;
using Refit;
using Todo.API.Services;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddRefitClient<ITodoClient>()
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
