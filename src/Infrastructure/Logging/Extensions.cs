using Finacle.Infrastructure.Persistence;
using Finacle.Infrastructure.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    Controller = context.Request.Path.Value?.Split('/')[1].Split(50, 1),
                    EndPoint = context.Request.Path.Value?.Split(200, 1),
                    RequestDate = DateTime.UtcNow,
                    UserAgent = context.Request.Headers["User-Agent"].FirstOrDefault()?.Split(200, 1),
                    Userid = context.User.Identity?.Name?.Split(50, 1),
                    TraceId = context.TraceIdentifier.Split(50, 1)
                };

                try
                {
                    var dbContext = context.RequestServices.GetRequiredService<FinacleDbContext>();
                    dbContext.ApiCalls.Add(apiCall);
                    await dbContext.SaveChangesAsync();
                }
                catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2627) // SQL Server error code for unique constraint violation
                {
                    Log.Warning("Duplicate requestId detected: {RequestId}", requestId);
                }

                await next();

                // Update the response details after the request has been processed
                apiCall.ResponseCode = context.Response.StatusCode.ToString().Split(50, 1);
                apiCall.ResponseParam = context.Response.Headers.ToString()?.Split(200, 1);
                apiCall.ReponseTime = DateTime.UtcNow;

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
