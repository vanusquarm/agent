
using System.Text.Json.Serialization;

namespace Transactions.API.Entities.DTOs
{
    public class CashTransferRequest
    {
        [JsonPropertyName("transactionType")]
        public required string TransactionType { get; set; }

        [JsonPropertyName("transactionSubType")]
        public required string TransactionSubType { get; set; }

        [JsonPropertyName("transactionSummary")]
        public List<TransactionSummary>? TransactionSummary { get; set; }

        [JsonPropertyName("denominationDetails")]
        public List<DenominationDetail>? DenominationDetails { get; set; }

        [JsonPropertyName("userTransactionCode")]
        public string? UserTransactionCode { get; set; }

        [JsonPropertyName("transactionRemarks")]
        public string? TransactionRemarks { get; set; }

        [JsonPropertyName("entererId")]
        public string? EntererId { get; set; }
    }

    public class TransactionSummary
    {
        [JsonPropertyName("serialNumber")]
        public string? SerialNumber { get; set; }

        [JsonPropertyName("partTransactionType")]
        public string? PartTransactionType { get; set; }

        [JsonPropertyName("accountIdentification")]
        public string? AccountIdentification { get; set; }

        [JsonPropertyName("transactionAmount")]
        public decimal TransactionAmount { get; set; }

        [JsonPropertyName("transactionCurrency")]
        public string? TransactionCurrency { get; set; }

        [JsonPropertyName("valuedDate")]
        public string? ValuedDate { get; set; }

        [JsonPropertyName("printAdvice")]
        public string? PrintAdvice { get; set; }

        [JsonPropertyName("transactionParticulars")]
        public string? TransactionParticulars { get; set; }
    }

    public class DenominationDetail
    {
        [JsonPropertyName("denominationDetailsCurrencyType")]
        public List<DenominationDetailsCurrencyType>? DenominationDetailsCurrencyType { get; set; }

        [JsonPropertyName("roundOffIndicator")]
        public string? RoundOffIndicator { get; set; }

        [JsonPropertyName("roundOffAmount")]
        public RoundOffAmount? RoundOffAmount { get; set; }

        [JsonPropertyName("denominationRemarks")]
        public string? DenominationRemarks { get; set; }
    }

    public class DenominationDetailsCurrencyType
    {
        [JsonPropertyName("denominationCurrency")]
        public string? DenominationCurrency { get; set; }

        [JsonPropertyName("denomination")]
        public int Denomination { get; set; }

        [JsonPropertyName("denominationType")]
        public string? DenominationType { get; set; }

        [JsonPropertyName("strapOrBundleSize")]
        public int StrapOrBundleSize { get; set; }

        [JsonPropertyName("numberOfNotesOrCoins")]
        public int NumberOfNotesOrCoins { get; set; }

        [JsonPropertyName("numberOfStrapsOrBundles")]
        public int NumberOfStrapsOrBundles { get; set; }

        [JsonPropertyName("numberOfNotesOrCoinsPaybackOrGetback")]
        public int NumberOfNotesOrCoinsPaybackOrGetback { get; set; }

        [JsonPropertyName("numberOfStrapsOrBundlesPaybackOrGetback")]
        public int NumberOfStrapsOrBundlesPaybackOrGetback { get; set; }
    }

    public class RoundOffAmount
    {
        [JsonPropertyName("value")]
        public decimal Value { get; set; }

        [JsonPropertyName("currency")]
        public string? Currency { get; set; }
    }
}

public class CashTransferResponse
{
    [JsonPropertyName("transactionDate")]
    public string? TransactionDate { get; set; }

    [JsonPropertyName("transactionStatus")]
    public string? TransactionStatus { get; set; }

    [JsonPropertyName("transactionIdentification")]
    public string? TransactionIdentification { get; set; }

    [JsonPropertyName("tranMessage")]
    public string? TranMessage { get; set; }
}

public class PerformCashTransferRequest
{
    public string TellerId { get; set; }
    public string CustomerAccount { get; set; }
    public decimal Amount { get; set; }
    public string Remarks { get; set; }
    public string ChannelReference { get; set; }
    public string TransType { get; set; }
    public string? Currency { get; set; }
    public Dictionary<string, string>? DenomDetails { get; set; }
}

public static class TransactionType
{
    public const string WITHDRAWAL = "D"; // Debit customer account, Credit Teller Cash Account
    public const string DEPOSIT = "C";    // Credit customer account, Debit Teller Cash Account
}