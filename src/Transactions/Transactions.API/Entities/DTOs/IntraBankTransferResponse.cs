namespace Transactions.API.Entities.DTOs;

public class IntraBankTransferResponse
{
    public DateTime TransactionDate { get; set; }
    public string TransactionReference { get; set; }
}