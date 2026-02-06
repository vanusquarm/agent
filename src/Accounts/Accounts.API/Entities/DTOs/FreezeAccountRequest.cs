using System.Text.Json.Serialization;

namespace Accounts.API.Entities.DTOs
{
    public class FreezeAccountRequest
    {
        [JsonPropertyName("customerId")]
        public required string CustomerId { get; set; }

        [JsonPropertyName("accountNumber")]
        public required string AccountNumber { get; set; }

        [JsonPropertyName("reasonType")]
        public string ReasonType { get; set; } = "D";

        [JsonPropertyName("reasonCode")] 
        public string ReasonCode { get; set; } = "001";

        [JsonPropertyName("freezeRemarks")]
        public string? FreezeRemarks { get; set; }
    }


    public class UnFreezeAccountRequest
    {
        [JsonPropertyName("customerId")]
        public required string CustomerId { get; set; }

        [JsonPropertyName("reasonCode")]
        public string ReasonCode { get; set; } = "001";

        [JsonPropertyName("remarks")]
        public string? Remarks { get; set; }
    }
}
