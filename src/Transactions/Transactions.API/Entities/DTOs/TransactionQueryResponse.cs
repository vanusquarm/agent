using System.Text.Json.Serialization;

namespace Transactions.API.Entities.DTOs;

public class TransactionQueryResponse
{
    public string AccountIdentification { get; set; }
    public List<TransactionDetail> Transactions { get; set; }
}

public class GetTransactionsResponse
{
    [JsonPropertyName("accountIdentification")]
    public string AccountIdentification { get; set; }

    [JsonPropertyName("productCode")]
    public string ProductCode { get; set; }

    [JsonPropertyName("productDescription")]
    public string ProductDescription { get; set; }

    [JsonPropertyName("fromDate")]
    public string FromDate { get; set; }

    [JsonPropertyName("toDate")]
    public string ToDate { get; set; }

    [JsonPropertyName("tranDetails")]
    public List<TranDetail> TranDetails { get; set; }
}

public class TranDetail
{
    [JsonPropertyName("branchCode")]
    public string BranchCode { get; set; }

    [JsonPropertyName("transactionCurrency")]
    public string TransactionCurrency { get; set; }

    [JsonPropertyName("transactionIdentification")]
    public string TransactionIdentification { get; set; }

    [JsonPropertyName("transactionDate")]
    public string TransactionDate { get; set; }

    [JsonPropertyName("transactionType")]
    public string TransactionType { get; set; }

    [JsonPropertyName("transactionSubType")]
    public string TransactionSubType { get; set; }

    [JsonPropertyName("transactionAmount")]
    public string TransactionAmount { get; set; }

    [JsonPropertyName("DebitCreditIndicator")]
    public string DebitCreditIndicator { get; set; }

    [JsonPropertyName("valueDate")]
    public string ValueDate { get; set; }

    [JsonPropertyName("transactionReference")]
    public string TransactionReference { get; set; }

    [JsonPropertyName("currencyDescription")]
    public string CurrencyDescription { get; set; }

    [JsonPropertyName("branchName")]
    public string BranchName { get; set; }

    [JsonPropertyName("instrumentNo")]
    public string InstrumentNo { get; set; }

    [JsonPropertyName("instrumentType")]
    public string InstrumentType { get; set; }

    [JsonPropertyName("instrumentDate")]
    public string InstrumentDate { get; set; }

    [JsonPropertyName("SerialNum")]
    public string SerialNum { get; set; }

    [JsonPropertyName("postedTime")]
    public string PostedTime { get; set; }

    [JsonPropertyName("BankCode")]
    public string BankCode { get; set; }

    [JsonPropertyName("OriginatingBankCode")]
    public string OriginatingBankCode { get; set; }

    [JsonPropertyName("transactionRemarks")]
    public string TransactionRemarks { get; set; }

    [JsonPropertyName("contraAccount")]
    public string ContraAccount { get; set; }

    [JsonPropertyName("contraAmount")]
    public string ContraAmount { get; set; }

    [JsonPropertyName("contraTrantype")]
    public string ContraTrantype { get; set; }

    [JsonPropertyName("contraAccountName")]
    public string ContraAccountName { get; set; }

    [JsonPropertyName("reversalInd")]
    public string ReversalInd { get; set; }

    [JsonPropertyName("userTranCode")]
    public string UserTranCode { get; set; }

    [JsonPropertyName("userTranDescription")]
    public string UserTranDescription { get; set; }

    [JsonPropertyName("stan")]
    public string Stan { get; set; }
}

