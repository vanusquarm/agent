using System.Text.Json.Serialization;

namespace Finacle.Core.Domain
{
    public class ReversalTransferRequest
    {
        [JsonPropertyName("transactionDate")]
        public string TransactionDate { get; set; }

        [JsonPropertyName("transactionReference")]
        public  string TransactionReference { get; set; }

        [JsonPropertyName("remarks")]
        public string Remarks { get; set; }
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
