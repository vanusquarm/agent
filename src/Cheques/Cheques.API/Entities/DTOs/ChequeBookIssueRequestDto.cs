using System.Text.Json.Serialization;

namespace Cheques.API.Entities.DTOs
{
    public class ChequeBookIssueRequestDto
    {
        [JsonPropertyName("numberOfChequeBooks")]
        public int NumberOfChequeBooks { get; set; }

        [JsonPropertyName("chequeBookNumber")]
        public string ChequeBookNumber { get; set; } = string.Empty;

        [JsonPropertyName("startChequeNumber")]
        public string StartChequeNumber { get; set; } = string.Empty;

        [JsonPropertyName("endChequeNumber")]
        public string EndChequeNumber { get; set; } = string.Empty;

        [JsonPropertyName("issuanceDate")]
        public DateTime IssuanceDate { get; set; }

        [JsonPropertyName("chargesApplicable")]
        public string ChargesApplicable { get; set; } = string.Empty;

    }
}
