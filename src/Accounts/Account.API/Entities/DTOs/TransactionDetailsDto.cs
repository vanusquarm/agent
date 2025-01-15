using System.Text.Json.Serialization;

namespace Acccount.API.Entities.DTOs
{
    public class TransactionDetailsDto
    {
        [JsonPropertyName("accountIdentification")]
        public string? AccountIdentification { get; set; }

        [JsonPropertyName("transactionId")]
        public string? TransactionId { get; set; }

        [JsonPropertyName("transactionDate")]
        public string? TransactionDate { get; set; }

        [JsonPropertyName("tranSerialNo")]
        public int? TranSerialNo { get; set; }

        [JsonPropertyName("debitCreditIndicator")]
        public string? DebitCreditIndicator { get; set; }

        [JsonPropertyName("transactionAmount")]
        public TransactionAmount? TransactionAmount { get; set; }

        [JsonPropertyName("transactionBalance")]
        public TransactionBalance? TransactionBalance { get; set; }

        [JsonPropertyName("instrumentId")]
        public string? InstrumentId { get; set; }

        [JsonPropertyName("transactionParticulars")]
        public string? TransactionParticulars { get; set; }

        [JsonPropertyName("transactionType")]
        public string? TransactionType { get; set; }

        [JsonPropertyName("transactionSubType")]
        public string? TransactionSubType { get; set; }

        [JsonPropertyName("postedDate")]
        public string? PostedDate { get; set; }

        [JsonPropertyName("valueDate")]
        public string? ValueDate { get; set; }

        [JsonPropertyName("channelIdentification")]
        public string? ChannelIdentification { get; set; }

        [JsonPropertyName("rate")]
        public string? Rate { get; set; }

        [JsonPropertyName("rateCode")]
        public string? RateCode { get; set; }

        [JsonPropertyName("partTransactionRemarks")]
        public string? PartTransactionRemarks { get; set; }

        [JsonPropertyName("userPartTransactionCode")]
        public string? UserPartTransactionCode { get; set; }

        [JsonPropertyName("referenceNumber")]
        public string? ReferenceNumber { get; set; }

        [JsonPropertyName("transactionParticularCode")]
        public string? TransactionParticularCode { get; set; }

        [JsonPropertyName("partTransactionRemarks2")]
        public string? PartTransactionRemarks2 { get; set; }

        [JsonPropertyName("referenceAmount")]
        public int? ReferenceAmount { get; set; }

        [JsonPropertyName("referenceCurrency")]
        public string? ReferenceCurrency { get; set; }

        [JsonPropertyName("todRequired")]
        public string? TodRequired { get; set; }
    }

    public class TransactionAmount
    {
        [JsonPropertyName("currency")]
        public string? Currency { get; set; }

        [JsonPropertyName("value")]
        public int? Value { get; set; }
    }

    public class TransactionBalance
    {
        [JsonPropertyName("currency")]
        public string? Currency { get; set; }

        [JsonPropertyName("value")]
        public int? Value { get; set; }
    }


}
