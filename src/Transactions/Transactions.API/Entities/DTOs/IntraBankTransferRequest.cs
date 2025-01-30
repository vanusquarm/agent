namespace Transactions.API.Entities.DTOs;


public class IntraBankTransferRequest
{
    public string TransactionType { get; set; }
    public string TransactionSubType { get; set; }
    public string ChannelIdentification { get; set; }
    public string EntererId { get; set; }
    public List<IntraBankTransferTransactionDetail> TransferTransactionDetail { get; set; }
}

public class IntraBankTransferTransactionDetail
{
    public string SerialNumber { get; set; }
    public string PartTransactionType { get; set; }
    public string AccountIdentification { get; set; }
    public decimal TransactionAmount { get; set; }
    public string TransactionCurrency { get; set; }
    public string TransactionValueDate { get; set; }
    public string TransactionParticulars { get; set; }
    public decimal Rate { get; set; }
    public string PartTransactionRemarks { get; set; }
    public string ReferenceNumber { get; set; }
    // public IntraBankMiscellaneousDetailsType MiscellaneousDetailsType { get; set; }
    public string PartTransactionRemarks2 { get; set; }
    public List<LienDetails> LienDetailsList { get; set; } = new();
}

public class IntraBankMiscellaneousDetailsType
{
    public string DocumentNumber { get; set; }
    public string FreeTxt2 { get; set; }
    public string FreeTxt3 { get; set; }
    public string FreeTxt4 { get; set; }
}

public class LienDetails
{
    public string ModuleType { get; set; }
    public string LienIdentification { get; set; }
    public decimal LienReleaseAmount { get; set; }
}
