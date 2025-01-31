using System.Text.Json.Serialization;

namespace Cheques.API.Entities.DTOs
{
    public class RevokeChequeResponseDto
    {
        [JsonPropertyName("chargesLevied")]
        public string ChargesLevied { get; set; } = string.Empty;
    }
}
