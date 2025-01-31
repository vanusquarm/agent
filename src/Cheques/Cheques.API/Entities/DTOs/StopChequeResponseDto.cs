using System.Text.Json.Serialization;

namespace Cheques.API.Entities.DTOs
{
    public class StopChequeResponseDto
    {
        [JsonPropertyName("stopPaymentChargesLevied")]
        public string StopPaymentChargesLevied { get; set; } = string.Empty;

        [JsonPropertyName("chequeDetails")]
        public List<ChequeDetailDto> ChequeDetails { get; set; } = new();

    }

    public class ChequeDetailDto
    {
        [JsonPropertyName("chequeNumber")]
        public string ChequeNumber { get; set; } = string.Empty;

        [JsonPropertyName("chequeStopPaymentStatus")]
        public string ChequeStopPaymentStatus { get; set; } = string.Empty;
    }
}
