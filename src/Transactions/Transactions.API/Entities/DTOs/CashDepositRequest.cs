namespace Transactions.API.Entities.DTOs;

public class CashDepositRequest
{
    public required string TellerID { get; set; }
    public required string DebitAccount { get; set; }
    public int Amount { get; set; }
    public string Remarks { get; set; }
    public string ChannelReference { get; set; }
    public string? Currency { get; set; }
    public List<DenominationDetailsCurrencyType>? DenomDetails { get; set; }
}


