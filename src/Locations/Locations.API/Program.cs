// using Finacle.Infrastructure;
using Refit;
using Todo.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// builder.AddInfrastructure(enableSwagger: enableSwagger);
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
