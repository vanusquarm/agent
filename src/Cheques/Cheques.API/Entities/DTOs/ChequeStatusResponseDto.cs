using System.Text.Json.Serialization;

namespace Cheques.API.Entities.DTOs
{
    public class ChequeStatusResponseDto
    {
        [JsonPropertyName("accountIdentification")]
        public string AccountIdentification { get; set; } = string.Empty;

        [JsonPropertyName("chequeStatus")]
        public string ChequeStatus { get; set; } = string.Empty;

        [JsonPropertyName("chequeStatusLastUpdatedOn")]
        public DateTime ChequeStatusLastUpdatedOn { get; set; }

    }
}
