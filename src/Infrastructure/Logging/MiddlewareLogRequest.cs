using Microsoft.AspNetCore.Http;

namespace Finacle.Infrastructure.Logging
{
    public class MiddlewareLogRequest
    {
        internal string channel;

        public string RequestID { get; internal set; }
        public string TraceIdentifer { get; internal set; }
        public string RequestParam { get; internal set; }
        public string APIService { get; internal set; }
        public string? Controller { get; internal set; }
        public PathString EndPoint { get; internal set; }
        public string IpAddress { get; internal set; }
        public string UserAgent { get; internal set; }
        public string ResponseParam { get; internal set; }
        public string HttpStatusCode { get; internal set; }
        public string ResponseCode { get; internal set; }
    }
}