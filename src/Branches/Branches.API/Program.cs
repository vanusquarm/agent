// using Finacle.Infrastructure;

using Finacle.Infrastructure.Auth.BasicJwt;
using Refit;
using Todo.API.Services;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;


// Add services to the container.
// builder.AddInfrastructure(enableSwagger: enableSwagger);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddJwtMiddleware<IBranchesClient>(configuration);

builder.Services.AddRefitClient<IBranchesClient>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(builder.Configuration["BaseUrl"]); 
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
