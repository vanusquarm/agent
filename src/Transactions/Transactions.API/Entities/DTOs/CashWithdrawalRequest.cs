using System.Text.Json.Serialization;

namespace Transactions.API.Entities.DTOs;

public class CashWithdrawalRequest
{
    [JsonPropertyName("transactionType")]
    public required string TransactionType { get; set; } = "C";

    [JsonPropertyName("transactionSubType")]
    public required string TransactionSubType { get; set; } = "PI";

    [JsonPropertyName("transactionSummary")]
    public List<TransactionSummary>? TransactionSummary { get; set; }

    [JsonPropertyName("denominationDetails")]
    public List<DenominationDetail>? DenominationDetails { get; set; }

    [JsonPropertyName("userTransactionCode")]
    public string? UserTransactionCode { get; set; } = "102";

    [JsonPropertyName("transactionRemarks")]
    public string? TransactionRemarks { get; set; }

    [JsonPropertyName("entererId")]
    public string? EntererId { get; set; }
}

public class TransactionSummary
{
    [JsonPropertyName("serialNumber")]
    public string? SerialNumber { get; set; } = "001";

    [JsonPropertyName("partTransactionType")]
    public string? PartTransactionType { get; set; } = "D";

    [JsonPropertyName("accountIdentification")]
    public string? AccountIdentification { get; set; }

    [JsonPropertyName("transactionAmount")]
    public int TransactionAmount { get; set; }

    [JsonPropertyName("transactionCurrency")]
    public string? TransactionCurrency { get; set; } = "GHS";

    [JsonPropertyName("valuedDate")]
    public string? ValuedDate { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");

    [JsonPropertyName("printAdvice")]
    public string? PrintAdvice { get; set; }

    [JsonPropertyName("transactionParticulars")]
    public string? TransactionParticulars { get; set; } = "02";
}

public class DenominationDetail
{
    [JsonPropertyName("denominationDetailsCurrencyType")]
    public List<DenominationDetailsCurrencyType>? DenominationDetailsCurrencyType { get; set; } 

    [JsonPropertyName("roundOffIndicator")]
    public string? RoundOffIndicator { get; set; } = "C";

    [JsonPropertyName("roundOffAmount")]
    public RoundOffAmount? RoundOffAmount { get; set; } 

    [JsonPropertyName("denominationRemarks")]
    public string? DenominationRemarks { get; set; } = "ok";
}

public class DenominationDetailsCurrencyType
{
    [JsonPropertyName("denominationCurrency")]
    public string? DenominationCurrency { get; set; } = "GHS";

    [JsonPropertyName("denomination")]
    public int Denomination { get; set; } = 100;

    [JsonPropertyName("denominationType")]
    public string? DenominationType { get; set; } = "N";

    [JsonPropertyName("strapOrBundleSize")]
    public int StrapOrBundleSize { get; set; } = 100;

    [JsonPropertyName("numberOfNotesOrCoins")]
    public int NumberOfNotesOrCoins { get; set; } = 2;

    [JsonPropertyName("numberOfStrapsOrBundles")]
    public int NumberOfStrapsOrBundles { get; set; } = 0;

    [JsonPropertyName("numberOfNotesOrCoinsPaybackOrGetback")]
    public int NumberOfNotesOrCoinsPaybackOrGetback { get; set; } = 0;

    [JsonPropertyName("numberOfStrapsOrBundlesPaybackOrGetback")]
    public int NumberOfStrapsOrBundlesPaybackOrGetback { get; set; } = 0;
}

public class RoundOffAmount
{
    [JsonPropertyName("value")]
    public int Value { get; set; } = 0;

    [JsonPropertyName("currency")]
    public string? Currency { get; set; } = "GHS";
}
