using System.Text.Json.Serialization;

namespace Acccounts.API.Entities.DTOs
{
    public class AccountDto
    {
        [JsonPropertyName("accountIdentification")]
        public string AccountIdentification { get; set; }

        [JsonPropertyName("iBAN")]
        public string IBAN { get; set; }
    }
}
