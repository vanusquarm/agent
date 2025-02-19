using System.Text.Json.Serialization;

namespace Cheques.API.Entities.DTOs
{
    public class ChequeConfirmationRequest
    {
        [JsonPropertyName("accountId")]
        public required string AccountId { get; set; }

        [JsonPropertyName("mode")]
        public required string Mode { get; set; }

        [JsonPropertyName("chequeNo")]
        public required string ChequeNo { get; set; }

        [JsonPropertyName("chqDate")]
        public required string ChqDate { get; set; }

        [JsonPropertyName("chqAmt")]
        public required string ChqAmt { get; set; }

        [JsonPropertyName("beneficiary")]
        public required string Beneficiary { get; set; }

        [JsonPropertyName("confirmation")]
        public required string Confirmation { get; set; }

        [JsonPropertyName("remarks")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Remarks { get; set; }

        [JsonPropertyName("acctMngerId")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? AcctMngerId { get; set; }

        [JsonPropertyName("rmId")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? RmId { get; set; }
    }
}
