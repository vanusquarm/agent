namespace Transactions.API.Entities.DTOs;

public class CashDepositResponse
{
    public string TransactionId { get; set; }
    public string Status { get; set; }
    public string Message { get; set; }
}