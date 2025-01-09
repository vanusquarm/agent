using Notification.Entities;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<OtpDb>(opt => opt.UseInMemoryDatabase("CustomerOtps")); // In-memory Database

var app = builder.Build();

app.Run();
