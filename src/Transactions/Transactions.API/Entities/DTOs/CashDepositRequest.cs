namespace Transactions.API.Entities.DTOs;

public class CashDepositRequest
{
    public string TransactionType { get; set; }
    public string TransactionSubType { get; set; }
    public List<TransactionSummary> TransactionSummary { get; set; }
    public string UserTransactionCode { get; set; }
    public string TransactionRemarks { get; set; }
    public string EntererId { get; set; }
}

public class TransactionSummary
{
    public string SerialNumber { get; set; }
    public string PartTransactionType { get; set; }
    public string AccountIdentification { get; set; }
    public decimal TransactionAmount { get; set; }
    public string TransactionCurrency { get; set; }
    public string RateCode { get; set; }
    public decimal Rate { get; set; }
    public string TreasuryReferenceNumber { get; set; }
    public decimal TreasuryRate { get; set; }
    public DateTime ValuedDate { get; set; }
    public string PrintAdvice { get; set; }
    public string TransactionParticularsCode { get; set; }
    public string ChargeEventId { get; set; }
    public List<ChargeDetails> ChargeDetails { get; set; }
}

public class ChargeDetails
{
    public string ChargeSerialNumber { get; set; }
    public decimal ChargeAmount { get; set; }
}

