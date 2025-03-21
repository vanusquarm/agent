using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using System.Diagnostics;
using System.Text;
using System.Net.Sockets;
using Polly;

namespace Finacle.Infrastructure.Logging
{
    public class RequestResponseLoggerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggerMiddleware> _logger;
        private readonly IConfiguration _config;
        private readonly IMiddlewareLog _logRequest;
        private readonly bool _isRequestResponseLoggingEnabled;
        StringBuilder _strBuilder = null;

        public RequestResponseLoggerMiddleware(RequestDelegate next, ILogger<RequestResponseLoggerMiddleware> logger)
        {
            _next = next;
            _logger = logger;

        }

        public async Task InvokeAsync(HttpContext httpContext, IConfiguration config, IMiddlewareLog logRequestResponse)
        {


            _strBuilder = new StringBuilder();

            MiddlewareLogRequest middlewareLogRequest = new();
            var uri = httpContext.Request.Path.ToString();

            _logger.LogInformation("Encrypted RequestId:{requestId}, Channel:{channel}", httpContext.Request.Headers["RequestId"].ToString(), httpContext.Request.Headers["ChannelId"].ToString());
            middlewareLogRequest.RequestID = httpContext.Request.Headers["RequestId"].ToString();

            _logger.LogInformation("Decrypted RequestId:{requestId}, Channel:{channel}", middlewareLogRequest.RequestID, httpContext.Request.Headers["ChannelId"].ToString());
            middlewareLogRequest.TraceIdentifer = httpContext.TraceIdentifier;

            middlewareLogRequest.RequestParam = await ReadBodyFromRequest(httpContext.Request);

            middlewareLogRequest.channel = httpContext.Request.Headers["ChannelId"].ToString();


            if (!uri.StartsWith("/swagger"))
            {
                _logger.LogInformation($"Http Request Information:{Environment.NewLine}" +
                              $"Schema:{httpContext.Request.Scheme} " +
                              $"Host: {httpContext.Request.Host} " +
                              $"Path: {httpContext.Request.Path} " +
                              $"Method: {httpContext.Request.Method} " +
                              $"QueryString: {httpContext.Request.QueryString} " +
                              $"Headers: {JsonConvert.SerializeObject(httpContext.Request.Headers)}" +
                              $"Request Body: {middlewareLogRequest.RequestParam}" +
                              $"Trace Identifier: {httpContext.TraceIdentifier}" +
                              $"Activity_Trace_Id: {Activity.Current.TraceId.ToString()}");

                middlewareLogRequest.APIService = "TransferService_CBA";
                middlewareLogRequest.Controller = httpContext.Request.RouteValues["controller"]?.ToString();
                middlewareLogRequest.EndPoint = httpContext.Request.Path;
                middlewareLogRequest.IpAddress = httpContext.Connection.RemoteIpAddress.ToString();
                middlewareLogRequest.IpAddress = GetIPAddress(httpContext);
                middlewareLogRequest.UserAgent = httpContext.Request.Headers.UserAgent.ToString();


                _strBuilder.AppendLine($"Step 1. Initializing....Http Request Information:{Environment.NewLine}" +
                            $"Schema:{httpContext.Request.Scheme} " +
                            $"Host: {httpContext.Request.Host} " +
                            $"Path: {httpContext.Request.Path} " +
                            $"Method: {httpContext.Request.Method} " +
                            $"QueryString: {httpContext.Request.QueryString} " +
                            $"Headers: {JsonConvert.SerializeObject(httpContext.Request.Headers)} " +
                            $"Request Body: {middlewareLogRequest.RequestParam} " +
                            $"Trace Identifier: {httpContext.TraceIdentifier} " +
                            $"Activity_Trace_Id:{Activity.Current.TraceId.ToString()} " +
                            $"RequestId:{httpContext.Request.Headers["requestId"].FirstOrDefault()}" +
                            $"Log Object:{JsonConvert.SerializeObject(middlewareLogRequest)}");


                var insertRes = await logRequestResponse.LogRequestResponse(middlewareLogRequest);
                if (insertRes == "-99999")
                {
                    _logger.LogInformation("Flagged Transaction as Duplicate");
                    var errordetails = CustomProblemDetails.FailureResponse(detail: "Duplicate Transaction", ResponseCode: ResponseManager.DuplicateRecord.Item1, title: ResponseManager.FinacleFailure.Item2, status: (int)HttpStatusCode.BadRequest);
                    string response = JsonConvert.SerializeObject(errordetails);
                    httpContext.Response.StatusCode = (int) errordetails.Status;
                    await httpContext.Response.WriteAsync(response);
                    return;
                }
            }

            var originalResponseBody = httpContext.Response.Body;

            using var newResponseBody = new MemoryStream();
            httpContext.Response.Body = newResponseBody;
            try
            {
                // Call the next middleware in the pipeline  
                await _next(httpContext);
                //call to the middleware is back


            }
            finally
            {
                middlewareLogRequest.ResponseCode = httpContext.Response.StatusCode.ToString();
                newResponseBody.Seek(0, SeekOrigin.Begin);

                var responseBodyText = await new StreamReader(newResponseBody).ReadToEndAsync();

                if (!uri.StartsWith("/swagger"))
                {
                    _logger.LogInformation($"Http Reponse Information:{Environment.NewLine}" +
                                          $"ResponseParam:{JsonConvert.SerializeObject(responseBodyText)} " +
                                          $"ResponseCode: {httpContext.Response.StatusCode.ToString()} " +
                                          $"RequestID: {httpContext.Request.Headers["requestId"].FirstOrDefault()}" +
                                          $"Trace Identifier:  {httpContext.TraceIdentifier}" +
                                          $"Activity_Trace_Id:{Activity.Current.TraceId.ToString()}");

                    _strBuilder.AppendLine($"Step 2. Ending.....Http Reponse Information:{Environment.NewLine}" +
                                          $"ResponseParam:{JsonConvert.SerializeObject(responseBodyText)} " +
                                          $"ResponseCode: {httpContext.Response.StatusCode.ToString()} " +
                                          $"Trace Identifier:  {httpContext.TraceIdentifier} " +
                                          $"Activity_Trace_Id:{Activity.Current.TraceId.ToString()} " +
                                          $"Log Object:{JsonConvert.SerializeObject(middlewareLogRequest)}");

                    middlewareLogRequest.ResponseParam = JsonConvert.SerializeObject(responseBodyText);
                    middlewareLogRequest.HttpStatusCode = Convert.ToInt32(middlewareLogRequest.ResponseCode).ToString();
                    middlewareLogRequest.RequestParam = httpContext.Items["RequestPayload"] as dynamic;
                    middlewareLogRequest.TraceIdentifer = httpContext.TraceIdentifier;
                    //middlewareLogRequest.RequestID = await EncryptionHelper.RSADecrypt(httpContext.Request.Headers["RequestId"].ToString(), httpContext.Request.Headers["ChannelId"].ToString());

                    _logger.LogInformation(_strBuilder.ToString());
                    logRequestResponse.UpdateLogRequestResponse(middlewareLogRequest);

                }

                newResponseBody.Seek(0, SeekOrigin.Begin);
                await newResponseBody.CopyToAsync(originalResponseBody);
                //_logger.LogError(ex, "An error occurred at RequestResponseLoggerMiddleware");
                //var errordetails = CustomProblemDetails.FailureResponse(detail: "Request/Response error", ResponseCode: ResponseManager.Exception.Item1, title: ResponseManager.Exception.Item2, status: (int)HttpStatusCode.BadRequest);
                //string response = JsonConvert.SerializeObject(errordetails);
                //httpContext.Response.StatusCode = (int)errordetails.Status;
                //await httpContext.Response.WriteAsync(response);

                //throw;
                //return;
            }

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

    }
}
