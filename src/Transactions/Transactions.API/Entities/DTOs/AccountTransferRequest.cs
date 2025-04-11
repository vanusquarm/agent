namespace Transactions.API.Entities.DTOs;

public class AccountTransferRequest
{
    public string SourceAccountId { get; set; }
    public string DestinationAccountId { get; set; }
    public decimal Amount { get; set; }
    public string Remarks { get; set; }
    public string ChannelReference { get; set; }
    public string ChannelId { get; set; }
    public string? Currency { get; set; }
    public string? DocNum { get; set; }
    public string? UserPartTransactionCode { get; set; }
    public string? ChargeEventId { get; set; }
    public string? BatchId { get; set; }
    public void Trim()
    {
        SourceAccountId = SourceAccountId?.Trim();
        DestinationAccountId = DestinationAccountId?.Trim();
        Remarks = Remarks?.Trim();
        ChannelReference = ChannelReference?.Trim();
        ChannelId = ChannelId?.Trim();
        DocNum = DocNum?.Trim();
        UserPartTransactionCode = UserPartTransactionCode?.Trim();
        ChargeEventId = ChargeEventId?.Trim();
        BatchId = BatchId?.Trim();
    }
}