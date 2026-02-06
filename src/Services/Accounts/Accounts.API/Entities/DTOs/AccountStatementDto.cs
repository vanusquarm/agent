using System.Text.Json.Serialization;

namespace Accounts.API.Entities.DTOs
{
    public class AccountStatementRequest
    {
        //[JsonPropertyName("accountId")]
        //public string? AccountId { get; set; }

        [JsonPropertyName("fromDate")]
        public required string FromDate { get; set; }

        [JsonPropertyName("toDate")]
        public required string ToDate { get; set; }

        [JsonPropertyName("setNumber")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? SetNumber { get; set; }

        [JsonPropertyName("maximumRecords")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? MaximumRecords { get; set; }
    }

    public class AcctStatementResponse
    {
        [JsonPropertyName("accountDetails")]
        public AccountDetails AccountDetails { get; set; }

        [JsonPropertyName("openingBalance")]
        public decimal OpeningBalance { get; set; }

        [JsonPropertyName("closingBalance")]
        public decimal ClosingBalance { get; set; }

        [JsonPropertyName("statementRecords")]
        public List<StatementRecord> StatementRecords { get; set; }

        [JsonPropertyName("isLastSet")]
        public string IsLastSet { get; set; }

        [JsonPropertyName("setNumber")]
        public int SetNumber { get; set; }
    }

    public class AccountDetails
    {
        [JsonPropertyName("accountIdentification")]
        public string AccountIdentification { get; set; }

        [JsonPropertyName("accountName")]
        public string AccountName { get; set; }

        [JsonPropertyName("customerReference")]
        public string CustomerReference { get; set; }

        [JsonPropertyName("currencyCode")]
        public string CurrencyCode { get; set; }
    }

    public class StatementRecord
    {
        [JsonPropertyName("transactionReference")]
        public string TransactionReference { get; set; }

        [JsonPropertyName("transactionDate")]
        public string TransactionDate { get; set; }

        [JsonPropertyName("transactionType")]
        public string TransactionType { get; set; }

        [JsonPropertyName("transactionValueDate")]
        public string TransactionValueDate { get; set; }

        [JsonPropertyName("transactionAmount")]
        public decimal TransactionAmount { get; set; }

        [JsonPropertyName("balance")]
        public decimal Balance { get; set; }

        [JsonPropertyName("tranParticulars")]
        public string TranParticulars { get; set; }
    }
}
