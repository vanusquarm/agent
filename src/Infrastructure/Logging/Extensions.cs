using Finacle.Infrastructure.Persistence;
using Finacle.Infrastructure.Utils;
using Microsoft.AspNetCore.Builder;


using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;


namespace Finacle.Infrastructure.Logging
{
    public static class Extensions
    {
        public static void UseRequestResponseLoggerMiddleware(this IApplicationBuilder app)
        {
            app.UseHttpLogging(); // Add this line to enable HTTP logging

            // Middleware to log ApiCall
            app.Use(async (context, next) =>
            {
                var requestId = context.Request.Headers["requestId"].FirstOrDefault();
                var apiCall = new ApiCall
                {
                    RequestId = requestId ?? Guid.NewGuid().ToString(),
                    Channel = context.Request.Headers["Channel"].FirstOrDefault()?.Split(50, 1),
                    RequestParam = context.Request.QueryString.Value?.Split(200, 1),
                    IpAddress = context.Connection.RemoteIpAddress?.ToString().Split(50, 1),
                    Controller = context.Request.RouteValues["controller"]?.ToString().Split(50, 1),
                    EndPoint = context.Request.Path.Value?.Split(200, 1),
                    RequestDate = DateTime.UtcNow,
                    UserAgent = context.Request.Headers["User-Agent"].FirstOrDefault()?.Split(200, 1),
                    Userid = context.User.Identity?.Name?.Split(50, 1),
                    TraceId = context.TraceIdentifier.Split(50, 1)
                };

                try
                {
                    if (!context.Request.Path.ToString().StartsWith("/swagger"))
                    {
                        Log.Information("About to insert into ApiCall table with parameters: {@Request}", apiCall);
                        var dbContext = context.RequestServices.GetRequiredService<FinacleDbContext>();
                        dbContext.ApiCalls.Add(apiCall);
                        await dbContext.SaveChangesAsync();
                    }
                }
                catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2627) // SQL Server error code for unique constraint violation
                {
                    Log.Warning("Duplicate requestId detected: {RequestId}", requestId);
                }

                // Capture the response body
                var originalBodyStream = context.Response.Body;
                using var responseBody = new MemoryStream();
                context.Response.Body = responseBody;

                await next();

                // Update the response details after the request has been processed
                apiCall.ResponseCode = context.Response.StatusCode.ToString().Split(50, 1);
                responseBody.Seek(0, SeekOrigin.Begin);
                var responseText = await new StreamReader(responseBody).ReadToEndAsync();
                apiCall.ResponseParam = responseText.Split(200, 1);
                apiCall.ReponseTime = DateTime.UtcNow;

                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);
                context.Response.Body = originalBodyStream;

                try
                {
                    var dbContext = context.RequestServices.GetRequiredService<FinacleDbContext>();
                    dbContext.ApiCalls.Update(apiCall);
                    await dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error updating ApiCall log");
                }
            });
        }
    }
}
