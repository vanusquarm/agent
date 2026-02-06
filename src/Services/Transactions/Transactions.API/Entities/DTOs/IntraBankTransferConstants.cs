namespace Transactions.API.Entities.DTOs;

public class IntraBankTransferConstants
{
    public const string TransactionType = "T";
    public const string TransactionSubType = "CI";
    public const string ChannelIdentification = "BPS";
    public const string SerialNumberDebit = "1";
    public const string SerialNumberCredit = "2";
    public const string PartTransactionTypeDebit = "D";
    public const string PartTransactionTypeCredit = "C";
    public const string TransactionCurrency = "GHS";
    public const decimal Rate = 0.0m;
    public static readonly string TransactionValueDate = DateTime.Now.ToString("yyyy-MM-dd");
    public static string? UserTransactionCode { get; set; } = "102";
}