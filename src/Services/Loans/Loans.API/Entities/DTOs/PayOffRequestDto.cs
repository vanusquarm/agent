using System.Text.Json.Serialization;

namespace Loans.API.Entities.DTOs
{
    public class PayoffRequestDTO
    {
        [JsonPropertyName("transactionType")]
        public string TransactionType { get; set; } // e.g., "C"
        [JsonPropertyName("payoffValueDate")]
        public string PayoffValueDate { get; set; } // Date when the payoff should be applied

        [JsonPropertyName("collectionAccountIdentification")]
        public string CollectionAccountIdentification { get; set; } // Identifier for the collection account
    }

}
