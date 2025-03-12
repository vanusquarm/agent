using System.Text.Json.Serialization;

namespace Loans.API.Entities.DTOs
{
    public class PayOffResponseDto
    {
        [JsonPropertyName("accountIdentification")]
        public string AccountIdentification { get; set; }

        [JsonPropertyName("transactionReference")]
        public string TransactionReference { get; set; }

        [JsonPropertyName("transactionDate")]
        public DateTime TransactionDate { get; set; }
    }

}
