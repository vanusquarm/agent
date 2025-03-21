namespace Finacle.Infrastructure.Logging
{
    public interface IMiddlewareLog
    {
        Task<string> LogRequestResponse(MiddlewareLogRequest middlewareLogRequest);
        void UpdateLogRequestResponse(MiddlewareLogRequest middlewareLogRequest);
    }
}