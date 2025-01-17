using Finacle.Infrastructure;
using Customers.API.Services;
using Refit;
using Finacle.Infrastructure.Auth.BasicJwt;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.
// builder.AddInfrastructure(enableSwagger: enableSwagger);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddJwtMiddleware<ICustomerClient>(configuration);

var app = builder.Build();
//app.UseInfrastructure(builder.Environment, enableSwagger: true);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
