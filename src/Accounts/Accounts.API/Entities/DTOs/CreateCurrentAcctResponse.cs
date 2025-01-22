using System.Text.Json.Serialization;

namespace Acccounts.API.Entities.DTOs
{
    public class CreateCurrentAcctResponse
    {
        [JsonPropertyName("accountIdentification")]
        public string AccountIdentification { get; set; }

        [JsonPropertyName("iBAN")]
        public string IBAN { get; set; }
    }

}
