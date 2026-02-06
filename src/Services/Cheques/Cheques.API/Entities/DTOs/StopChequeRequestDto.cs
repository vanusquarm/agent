using System.Text.Json.Serialization;

namespace Cheques.API.Entities.DTOs
{
    public class StopChequeRequestDto
    {
        [JsonPropertyName("singleChequeMultipleCheque")]
        public string SingleChequeMultipleCheque { get; set; } = string.Empty;

        [JsonPropertyName("beginChequeNumber")]
        public string BeginChequeNumber { get; set; } = string.Empty;

        [JsonPropertyName("endChequeNumber")]
        public string EndChequeNumber { get; set; } = string.Empty;

        [JsonPropertyName("reasonReference")]
        public string ReasonReference { get; set; } = string.Empty;

        [JsonPropertyName("chequeDate")]
        public DateTime ChequeDate { get; set; }

        [JsonPropertyName("chequeAmount")]
        public string ChequeAmount { get; set; } = string.Empty;

        [JsonPropertyName("chequeIssuedTo")]
        public string ChequeIssuedTo { get; set; } = string.Empty;

        [JsonPropertyName("stopPayMode")]
        public string StopPayMode { get; set; } = string.Empty;

        [JsonPropertyName("validFrom")]
        public DateTime ValidFrom { get; set; }

        [JsonPropertyName("validTo")]
        public DateTime ValidTo { get; set; }

        [JsonPropertyName("waiveCharges")]
        public string WaiveCharges { get; set; } = string.Empty;

        [JsonPropertyName("waiverReasonReference")]
        public string WaiverReasonReference { get; set; } = string.Empty;

        [JsonPropertyName("remarks")]
        public string Remarks { get; set; } = string.Empty;
    }
}
