using System.Text.Json.Serialization;

namespace Cheques.API.Entities.DTOs
{
    public class RevokeChequeRequestDto
    {
        [JsonPropertyName("singleChequeMultipleCheque")]
        public string SingleChequeMultipleCheque { get; set; } = string.Empty;

        [JsonPropertyName("beginChequeNumber")]
        public string BeginChequeNumber { get; set; } = string.Empty;

        [JsonPropertyName("endChequeNumber")]
        public string EndChequeNumber { get; set; } = string.Empty;

        [JsonPropertyName("revokeReasonReference")]
        public string RevokeReasonReference { get; set; } = string.Empty;

        [JsonPropertyName("waiveCharges")]
        public string WaiveCharges { get; set; } = string.Empty;

        [JsonPropertyName("waiverReasonReference")]
        public string WaiverReasonReference { get; set; } = string.Empty;

        [JsonPropertyName("remarks")]
        public string Remarks { get; set; } = string.Empty;
    }
}
