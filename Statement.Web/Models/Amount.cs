using System.Text.Json.Serialization;

namespace Statement.Web.Models
{
    public class Amount
    {
        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("value")]
        public double? Value { get; set; }
    }
    public class ChargesResponse
    {
        [JsonPropertyName("errorMsg")]
        public string errorMsg { get; set; }

        [JsonPropertyName("errorFlg")]
        public string errorFlg { get; set; }
        [JsonPropertyName("transactionId")]
        public string? transactionId { get; set; }
        [JsonPropertyName("chrgAmt")]
        public string? chrgAmt { get; set; }
        public string? message { get; set; }
    }
    public class IntraBankTransferRequest
    {
        public string SourceAccountId { get; set; }
        public string DestinationAccountId { get; set; }
        public decimal Amount { get; set; }
        public string Remarks { get; set; }
        public string Currency { get; set; }
        public string ChannelReference { get; set; }
        public string ChannelId { get; set; }
        public string DocNum { get; set; }
        public string UserPartTransactionCode { get; set; }
        public string ChargeEventId { get; set; }
    }
    public class IntraBankTransferResponse
    {
        public string TransactionDate { get; set; }
        public string TransactionReference { get; set; }
        public string TranStatus { get; set; }
    }
}
