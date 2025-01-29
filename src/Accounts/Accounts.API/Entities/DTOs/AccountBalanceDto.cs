using System.Text.Json.Serialization;

namespace Acccounts.API.Entities.DTOs
{
    public class AccountBalanceDto
    {
        [JsonPropertyName("accountIdentification")]
        public string? AccountIdentification { get; set; }

        [JsonPropertyName("accountCurrency")]
        public string? AccountCurrency { get; set; }

        [JsonPropertyName("accountName")]
        public string? AccountName { get; set; }

        [JsonPropertyName("accountNickName")]
        public string? AccountNickName { get; set; }

        [JsonPropertyName("balances")]
        public List<Balance>? Balances { get; set; }
    }

    public class Amount
    {
        [JsonPropertyName("currency")]
        public string? Currency { get; set; }

        [JsonPropertyName("value")]
        public decimal? Value { get; set; }
    }

    public class Balance
    {
        [JsonPropertyName("amount")]
        public Amount? Amount { get; set; }

        [JsonPropertyName("creditDebitIndicator")]
        public string? CreditDebitIndicator { get; set; }

        [JsonPropertyName("balanceType")]
        public string? BalanceType { get; set; }
    }
}
