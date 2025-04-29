using System.Text.Json.Serialization;

namespace Transactions.API.Entities.DTOs
{
    public class ReversalTransferRequest
    {
        [JsonPropertyName("transactionIdentification")]
        public TransactionIdentification? TransactionIdentification { get; set; }
    }

    public class TransactionIdentification
    {
        [JsonPropertyName("transactionDate")]
        public string? TransactionDate { get; set; }

        [JsonPropertyName("transactionReference")]
        public required string TransactionReference { get; set; }

        [JsonPropertyName("remarks")]
        public string? Remarks { get; set; }
    }

    public class ReversalTransferResponse
    {
        [JsonPropertyName("reversalTransactionIdentification")]
        public ReversalTransactionIdentification ReversalTransactionIdentification { get; set; }
    }

    public class ReversalTransactionIdentification
    {
        [JsonPropertyName("transactionDate")]
        public string TransactionDate { get; set; }

        [JsonPropertyName("transactionReference")]
        public string TransactionReference { get; set; }
    }
}
