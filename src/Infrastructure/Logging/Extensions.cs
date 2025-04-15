using Finacle.Infrastructure.Persistence;
using Finacle.Infrastructure.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Text.Json;


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
                var requestId = await GetRequestId(context.Request) ?? context.Request.Headers["requestId"].FirstOrDefault();
                var apiCall = new ApiCall
                {
                    RequestId = requestId,
                    Channel = context.Request.Headers["Channel"].FirstOrDefault()?.Split(50, 1),
                    RequestParam = await ReadBodyFromRequest(context.Request),
                    IpAddress = context.Connection.RemoteIpAddress?.ToString().Split(50, 1) ?? "No IP address found",
                    Controller = context.Request.RouteValues["controller"]?.ToString().Split(50, 1),
                    EndPoint = context.Request.Path.Value?.Split(4000, 1),
                    RequestDate = DateTime.UtcNow,
                    UserAgent = context.Request.Headers["User-Agent"].FirstOrDefault()?.Split(200, 1),
                    Userid = context.Request.Headers["requestId"].FirstOrDefault()?.Split(15, 1),
                    TraceId = context.TraceIdentifier.Split(50, 1),
                    Apiservice = "TransactionService"
                };

                if (context.Request.Path.ToString().StartsWith("/api/v1/transactions") && HttpMethods.IsPost(context.Request.Method))
                {
                    var dbContext = context.RequestServices.GetRequiredService<FinacleDbContext>();
                    try
                    {
                        Log.Information("About to insert into ApiCall table with parameters: {@Request}", apiCall);
                        dbContext.ApiCalls.Add(apiCall);
                        await dbContext.SaveChangesAsync();
                        Log.Information("Back from inserting into ApiCall table");

                    }
                    catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2601) // SQL Server error code for unique constraint violation
                    {
                        Log.Warning("Duplicate requestId detected: {@RequestId}", requestId);
                        dbContext.ChangeTracker.Clear();

                        apiCall = dbContext.ApiCalls.FirstOrDefault(item => item.RequestId == requestId);
                        if (apiCall?.ResponseCode == StatusCodes.Status200OK.ToString())
                        {
                            Log.Information("Cannot reprocess successful transaction parameters: {@Request}", apiCall);
                            return;
                        }
                        else
                        {
                            var apiCallDup = ApiCallDup.Map(apiCall);
                            Log.Information("About to insert into ApiCallDup table with parameters: {@Request}", apiCallDup);
                            dbContext.ApiCallDuplicates.Add(apiCallDup);
                            await dbContext.SaveChangesAsync();
                            Log.Information("Back from inserting into ApiCallDup table");

                            Log.Information("About to reprocess transaction with parameters: {@Request}", apiCall);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Error inserting to either ApiCall or ApiCallDup log");
                        return;
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
                        Log.Information("About to update ApiCall log with parameters: {@Request}", apiCall);
                        dbContext.ApiCalls.Update(apiCall);
                        await dbContext.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Error updating ApiCall log", ex.Message, ex.StackTrace);
                    }
                }
                await next();
            });
        }

        private static async Task<string> ReadBodyFromRequest(HttpRequest request)
        {
            // Ensure the request's body can be read multiple times (for the next middlewares in the pipeline).  
            request.EnableBuffering();

            using var streamReader = new StreamReader(request.Body, leaveOpen: true);
            var requestBody = await streamReader.ReadToEndAsync();

            // Reset the request's body stream position for next middleware in the pipeline.  
            request.Body.Position = 0;
            return requestBody;
        }

        private static async Task<string> GetRequestId(HttpRequest request)
        {
            // Ensure the request's body can be read multiple times (for the next middlewares in the pipeline).  
            var requestPayloadString = await ReadBodyFromRequest(request);

            // Request Payload integrity checks
            return requestPayloadString.ToHash();
        }
    }
}
