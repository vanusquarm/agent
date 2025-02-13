using System.Text.Json.Serialization;

namespace Transactions.API.Entities.DTOs;

public class IntraBankTransferResponse
{
    [JsonPropertyName("transactionDate")]
    public required string TransactionDate { get; set; }

    [JsonPropertyName("transactionReference")]
    public required string TransactionReference { get; set; }

    [JsonPropertyName("tranStatus")]
    public required string TranStatus { get; set; }
}