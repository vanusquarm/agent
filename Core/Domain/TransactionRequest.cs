using System.Text.Json.Serialization;

namespace Finacle.Core.Domain
{
    public class TransactionRequest
    {
        [JsonPropertyName("fromDate")]
        public  string FromDate { get; set; }

        [JsonPropertyName("toDate")]
        public  string ToDate { get; set; }

        [JsonPropertyName("accountId")]
        public  string AccountId { get; set; }

        [JsonPropertyName("remarks")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Remarks { get; set; }

        [JsonPropertyName("refNo")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string RefNo { get; set; }

        [JsonPropertyName("tranInd")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string TranInd { get; set; }

        [JsonPropertyName("revInd")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string RevInd { get; set; }

        [JsonPropertyName("tranId")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string TranId { get; set; }

        [JsonPropertyName("tranType")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string TranType { get; set; }

        [JsonPropertyName("userTransactionCode")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string UserTransactionCode { get; set; }

        [JsonPropertyName("currency")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Currency { get; set; }

        [JsonPropertyName("tellerId")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string TellerId { get; set; }

        [JsonPropertyName("tranRefNumber")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string TranRefNumber { get; set; }

        [JsonPropertyName("tranRmks")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string TranRmks { get; set; }
    }

}
