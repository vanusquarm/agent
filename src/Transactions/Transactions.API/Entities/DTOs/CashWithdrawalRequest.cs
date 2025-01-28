namespace Transactions.API.Entities.DTOs;

public class CashWithdrawalRequest
{
    public string TransactionType { get; set; }
    public string TransactionSubType { get; set; }
    public List<CashWithdrawalTransactionSummary> TransactionSummary { get; set; }
    public string UserTransactionCode { get; set; }
    public string TransactionRemarks { get; set; }
    public string EntererId { get; set; }
}

public class CashWithdrawalTransactionSummary
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
    public InstrumentDetails InstrumentDetails { get; set; }
    public string TransactionParticulars { get; set; }
    public string TransactionParticularsCode { get; set; }
    public MiscellaneousDetailsType MiscellaneousDetailsType { get; set; }
    public string ChargeEventId { get; set; }
    public List<ChargeDetails> ChargeDetails { get; set; }
}

public class InstrumentDetails
{
    public DateTime InstrumentDate { get; set; }
    public string InstrumentAlpha { get; set; }
    public string InstrumentNumber { get; set; }
    public string InstrumentType { get; set; }
}

public class MiscellaneousDetailsType
{
    public string UserAdditionalFreeField1 { get; set; }
    public string UserAdditionalFreeField2 { get; set; }
    public string UserAdditionalFreeField3 { get; set; }
    public string UserAdditionalFreeField4 { get; set; }
    public string UserPartTransactionCode { get; set; }
    public string ReportCode { get; set; }
    public string ReferenceNumber { get; set; }
    public string TransactionRemarks1 { get; set; }
    public string TransactionRemarks2 { get; set; }
}

