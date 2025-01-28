namespace Transactions.API.Entities.DTOs;

public class CashWithdrawalResponse
{
    public string TransactionIdentification { get; set; }
    public string TransactionStatus { get; set; }
    public string TransactionDate { get; set; }
}