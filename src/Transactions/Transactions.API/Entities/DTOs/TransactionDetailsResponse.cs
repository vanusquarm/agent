namespace Transactions.API.Entities.DTOs;

public class TransactionDetailsResponse
{
    public List<TransactionDetail> TransactionDetails { get; set; }
    public TransactionIdentification TransactionIdentification { get; set; }
}

public class TransactionDetail
{
    public string SerialNumber { get; set; }
    public string AccountIdentification { get; set; }
    public string TransactionCurrency { get; set; }
    public string TransactionAmount { get; set; }
    public string DebitCreditIndicator { get; set; }
    public string TransactionStatus { get; set; }
    public DateTime TransactionDate { get; set; }
    public DateTime ValueDate { get; set; }
    public string TransactionNarration { get; set; }
}

public class TransactionIdentification
{
    public string TransactionReference { get; set; }
    public DateTime TransactionDate { get; set; }
}