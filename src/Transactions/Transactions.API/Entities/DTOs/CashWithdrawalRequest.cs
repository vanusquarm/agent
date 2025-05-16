namespace Transactions.API.Entities.DTOs;

public class CashWithdrawalRequest
{
    public required string TellerID { get; set; }
    public required string CreditAccount { get; set; }
    public decimal Amount { get; set; }
    public string Remarks { get; set; }
    public string ChannelReference { get; set; }
    public string? Currency { get; set; }
    public string? SolId { get; set; }
    public List<DenominationDetailsCurrencyType>? DenomDetails { get; set; }
}
