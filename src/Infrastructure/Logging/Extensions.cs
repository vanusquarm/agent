using Finacle.Infrastructure.Persistence;
using Finacle.Infrastructure.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Net.Sockets;
using System.Net;
using System.Text.Json;



namespace Finacle.Infrastructure.Logging
{
    public static class Extensions
    {
        public static void UseRequestResponseLoggerMiddleware(this IApplicationBuilder app)
        {
            // Middleware to log ApiCall
            app.Use(async (context, next) =>
            {
                var requestBody = await ReadBodyFromRequest(context.Request);
                var requestId = context.Request.Headers["requestId"].FirstOrDefault() ?? requestBody.ToHash();
                context.Items["requestId"] = requestId;

                var apiCall = new ApiCall
                {
                    RequestId = requestId,
                    Channel = context.Request.Headers["ChannelId"].FirstOrDefault()?.Split(50, 1),
                    RequestParam = requestBody,
                    IpAddress = GetIPAddress(context),
                    Controller = context.Request.RouteValues["controller"]?.ToString().Split(50, 1),
                    EndPoint = context.Request.Path.Value?.Split(4000, 1),
                    RequestDate = DateTime.UtcNow,
                    UserAgent = context.Request.Headers["User-Agent"].FirstOrDefault()?.Split(200, 1),
                    Userid = context.Request.Headers["requestId"].FirstOrDefault()?.Split(15, 1),
                    TraceId = context.TraceIdentifier.Split(50, 1),
                    Apiservice = "TransactionService"
                };

                if (context.Request.Path.ToString().ToLower().Contains("reversals") && HttpMethods.IsPost(context.Request.Method)) { await next(); }

                else if (context.Request.Path.ToString().ToLower().Contains("/api/v1/transactions") && HttpMethods.IsPost(context.Request.Method))
                {
                    var dbContext = context.RequestServices.GetRequiredService<FinacleDbContext>();
                    try
                    {
                        Log.Information("About to insert into ApiCall table with parameters: {@Request}", apiCall);
                        dbContext.ApiCalls.Add(apiCall);
                        await dbContext.SaveChangesAsync();
                        Log.Information("Back from inserting into ApiCall table");
                    }
                    catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2601)
                    {
                        Log.Warning("Duplicate requestId detected: {@RequestId}", requestId);
                        dbContext.ChangeTracker.Clear();

                        apiCall = dbContext.ApiCalls.FirstOrDefault(item => item.RequestId == requestId);
                        if (apiCall?.ResponseCode == StatusCodes.Status200OK.ToString())
                        {
                            Log.Information("Cannot reprocess successful transaction parameters: {@Request}", apiCall);
                            var errordetails = CustomProblemDetails.FailureResponse(detail: "Duplicate Transaction", ResponseCode: ResponseManager.DuplicateRecord.Item1, title: ResponseManager.FinacleFailure.Item2, status: (int)HttpStatusCode.BadRequest);
                            string response = JsonSerializer.Serialize(errordetails);
                            context.Response.StatusCode = (int)errordetails.Status;
                            await context.Response.WriteAsync(response);
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

                    // Process the request and allow the response to be written
                    await next();

                    // Update the response details after the request has been processed
                    responseBody.Seek(0, SeekOrigin.Begin);
                    var responseText = await new StreamReader(responseBody).ReadToEndAsync();
                    apiCall.ResponseCode = context.Response.StatusCode.ToString().Split(50, 1);
                    apiCall.ResponseParam = responseText;
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
                else
                {
                    await next();
                }
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

        private static string GetIPAddress(HttpContext httpContext)
        {
            var IpAddress = string.Empty;

            var xForwardedIp = string.Empty;


            var forwardedHeader = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedHeader))
            {
                xForwardedIp = forwardedHeader.Split(',')[0];
            }

            string hostName = Dns.GetHostName();
            var hostEntry = Dns.GetHostEntry(hostName);

            var ipAddress = hostEntry.AddressList
                .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);



            if (!string.IsNullOrEmpty(ipAddress?.ToString()) && !string.IsNullOrEmpty(xForwardedIp))
            {
                IpAddress = $"{xForwardedIp} , {ipAddress?.ToString()}";

            }
            else if (!string.IsNullOrEmpty(ipAddress?.ToString()) && string.IsNullOrEmpty(xForwardedIp))
            {
                IpAddress = ipAddress?.ToString();
            }
            else if (string.IsNullOrEmpty(ipAddress?.ToString()) && !string.IsNullOrEmpty(xForwardedIp))
            {
                IpAddress = xForwardedIp;
            }
            else
            {
                IpAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "No IP address found";
            }

            return IpAddress;
        }

        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
        {
            app.UseForwardedHeaders();
            // serilog logging with client ip
            return app.UseSerilogRequestLogging(options =>
            {
                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    var remoteIp = httpContext.Connection.RemoteIpAddress?.ToString();
                    var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();

                    var clientIp = !string.IsNullOrWhiteSpace(forwardedFor)
                        ? forwardedFor.Split(',')[0].Trim()
                        : remoteIp;

                    diagnosticContext.Set("ClientIp", clientIp);
                };
            });
        }
    }
}
