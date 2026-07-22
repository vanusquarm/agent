using System.Text.Json.Serialization;

namespace Statement.Web.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
    public class StatementRequest
    {
        [JsonPropertyName("accountNumber")]
        public string AccountNumber { get; set; }

        [JsonPropertyName("startDate")]
        public DateTime StartDate { get; set; }

        [JsonPropertyName("endDate")]
        public DateTime EndDate { get; set; }

        [JsonPropertyName("debitCurrency")]
        public string? DebitCurrency { get; set; }
    }

    public class StatementEmailRequest
    {
        [JsonPropertyName("accountNumber")]
        public string AccountNumber { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("startDate")]
        public DateTime StartDate { get; set; }

        [JsonPropertyName("endDate")]
        public DateTime EndDate { get; set; }
    }

    public class StatementDataResponse{
        public List<SingleAccountStatementData> AccountsStatementData { get; set; }
    }
    public class SingleAccountStatementData
    {
        [JsonPropertyName("accountNumber")]
        public string AccountNumber { get; set; }

        [JsonPropertyName("customerName")]
        public string CustomerName { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("accountType")]
        public string AccountType { get; set; }

        [JsonPropertyName("branch")]
        public string Branch { get; set; }

        [JsonPropertyName("address")]
        public string Address { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("openingBalance")]
        public decimal OpeningBalance { get; set; }

        [JsonPropertyName("closingBalance")]
        public decimal ClosingBalance { get; set; }

        [JsonPropertyName("startDate")]
        public DateTime StartDate { get; set; }

        [JsonPropertyName("endDate")]
        public DateTime EndDate { get; set; }

        [JsonPropertyName("hasTransactions")]
        public bool HasTransactions { get; set; }

        [JsonPropertyName("transactionCount")]
        public int TransactionCount { get; set; }

        [JsonPropertyName("totalDebit")]
        public decimal TotalDebit { get; set; }

        [JsonPropertyName("totalCredit")]
        public decimal TotalCredit { get; set; }

        [JsonPropertyName("debitCount")]
        public int DebitCount { get; set; }

        [JsonPropertyName("creditCount")]
        public int CreditCount { get; set; }

        [JsonPropertyName("transactions")]
        public List<StatementRecord> Transactions { get; set; }
    }
}
