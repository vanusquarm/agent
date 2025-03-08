using System.Text.Json.Serialization;

namespace Transactions.API.Entities.DTOs
{
   

    public class ReversalTransferRequestDto
    {
        [JsonPropertyName("transactionDate")]
        public string TransactionDate { get; set; }

        [JsonPropertyName("transactionReference")]
        public string TransactionReference { get; set; }

        [JsonPropertyName("remarks")]
        public string Remarks { get; set; }

        public string ChannelReference { get; set; }

    }
}
