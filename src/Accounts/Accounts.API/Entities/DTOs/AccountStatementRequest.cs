using System.Text.Json.Serialization;

namespace Acccounts.API.Entities.DTOs
{
    public class AccountStatementRequest
    {
        [JsonPropertyName("accountId")]
        public string? AccountId { get; set; }

        [JsonPropertyName("fromDate")]
        public required string FromDate { get; set; }

        [JsonPropertyName("toDate")]
        public required string ToDate { get; set; }

        [JsonPropertyName("setNumber")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? SetNumber { get; set; }

        [JsonPropertyName("maximumRecords")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? MaximumRecords { get; set; }
    }


}
