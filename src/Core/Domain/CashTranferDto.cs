using System.Text.Json.Serialization;

namespace Finacle.Core.Domain
{
    public class CashTransferRequest
    {
        public string TellerId { get; set; }
        public string CustomerAccountId { get; set; }
        public decimal Amount { get; set; }
        public string Remarks { get; set; }
        public string ChannelReference { get; set; }
        public string Currency { get; set; }
        public string SolId { get; set; }
    }

    public class CashTransferResponse
    {
        [JsonPropertyName("transactionDate")]
        public string TransactionDate { get; set; }

        [JsonPropertyName("transactionStatus")]
        public string TransactionStatus { get; set; }

        [JsonPropertyName("transactionIdentification")]
        public string TransactionIdentification { get; set; }

        [JsonPropertyName("tranMessage")]
        public string TranMessage { get; set; }
    }

}