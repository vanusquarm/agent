using System.Text.Json.Serialization;

namespace Transactions.API.Entities.DTOs;

public class CashWithdrawalResponse
{
    [JsonPropertyName("transactionDate")]
    public string? TransactionDate { get; set; }

    [JsonPropertyName("transactionStatus")]
    public string? TransactionStatus { get; set; }

    [JsonPropertyName("transactionIdentification")]
    public string? TransactionIdentification { get; set; }

    [JsonPropertyName("tranMessage")]
    public string? TranMessage { get; set; }
}

