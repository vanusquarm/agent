using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Finacle.Core.Domain
{
    public class FreezeAccountRequest
    {
        [JsonPropertyName("customerId")]
        public string CustomerId { get; set; }

        [JsonPropertyName("accountNumber")]
        public string AccountNumber { get; set; }

        [JsonPropertyName("reasonType")]
        public string ReasonType { get; set; } = "D";

        [JsonPropertyName("reasonCode")] 
        public string ReasonCode { get; set; } = "001";

        [JsonPropertyName("freezeRemarks")]
        public string FreezeRemarks { get; set; }
    }


    public class UnFreezeAccountRequest
    {
        [JsonPropertyName("customerId")]
        public string CustomerId { get; set; }

        [JsonPropertyName("reasonCode")]
        public string ReasonCode { get; set; } = "001";

        [JsonPropertyName("remarks")]
        public string Remarks { get; set; }
    }

    // Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
    public class FreezeDetail
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }
    }

    public class FreezeAccountResponse
    {
        [JsonPropertyName("freezeDetails")]
        public List<FreezeDetail> FreezeDetails { get; set; }

        [JsonPropertyName("errorFlg")]
        public string ErrorFlg { get; set; }
    }

    public class UnfreezeAccountRequest
    {
        public string CustomerId { get; set; }
        public string ReasonCode { get; set; }
        public string Remarks { get; set; }
    }

    public class UnFreezeAccountResponse
    {
        [JsonPropertyName("UnfreezeDetails")]
        public List<UnfreezeDetail> UnfreezeDetails { get; set; }

        [JsonPropertyName("errorFlg")]
        public string ErrorFlg { get; set; }
    }

    public class UnfreezeDetail
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }
    }


}
