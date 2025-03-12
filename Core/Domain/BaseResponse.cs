using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Finacle.Core.Domain
{
    public class BaseResponse
    {
        public string ChannelId { get; set; }   // Application Name
        public string ChannelReference { get; set; } // Transaction ID
        public Status Status { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }

    public enum Status
    {
        FAILED,
        SUCCESS,
        PENDING
    }

    // Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
    public class ErrorDetail
    {
        [JsonPropertyName("errorCode")]
        public string ErrorCode { get; set; }

        [JsonPropertyName("errorMessage")]
        public string ErrorMessage { get; set; } = "";

        [JsonPropertyName("errorTag")]
        public string ErrorTag { get; set; }
    }

    public class Error
    {
        [JsonPropertyName("errorDetails")]
        public List<ErrorDetail> ErrorDetails { get; set; }

        [JsonPropertyName("warningDetails")]
        public List<object> WarningDetails { get; set; }

        [JsonPropertyName("exceptionDetails")]
        public List<object> ExceptionDetails { get; set; }
    }
}
