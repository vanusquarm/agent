namespace Transactions.API.Entities.DTOs;

public class TransactionQueryResponse
{
    public string AccountIdentification { get; set; }
    public List<TransactionDetail> Transactions { get; set; }
}