using System.Text.Json.Serialization;

namespace Cheques.API.Entities.DTOs
{
    public class CheckBookIssueResponseDto
    {
        [JsonPropertyName("accountIdentification")]
        public string AccountIdentification { get; set; } = string.Empty;
    }
}
