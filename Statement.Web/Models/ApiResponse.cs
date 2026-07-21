using System.Text.Json.Serialization;

namespace Statement.Web.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
    public class EmailStatementAPIRequest
    {
    }
        public class EmailStatementRequest
    {
        //[JsonPropertyName("cifId")]
        //public string CIF_ID { get; set; }

        //[JsonPropertyName("email")]
        //public string Email { get; set; }

        [JsonPropertyName("accounts")]
        public List<AccountInfo> Accounts { get; set; }

        [JsonPropertyName("startDate")]
        public DateTime StartDate { get; set; }

        [JsonPropertyName("endDate")]
        public DateTime EndDate { get; set; }

        //[JsonPropertyName("isCompliance")]
        //public bool IsCompliance { get; set; }

        //[JsonPropertyName("accountToDebit")]
        //public string AccountToDebit { get; set; }

        //[JsonPropertyName("debitCurrency")]
        //public string DebitCurrency { get; set; }
    }

    public class StatementDataRequest
    {
        [JsonPropertyName("accountNumber")]
        public string AccountNumber { get; set; }

        [JsonPropertyName("startDate")]
        public DateTime StartDate { get; set; }

        [JsonPropertyName("endDate")]
        public DateTime EndDate { get; set; }
    }

    public class MultipleStatementDataRequest
    {
        [JsonPropertyName("accountNumbers")]
        public List<string> AccountNumbers { get; set; }

        [JsonPropertyName("startDate")]
        public DateTime StartDate { get; set; }

        [JsonPropertyName("endDate")]
        public DateTime EndDate { get; set; }
    }

    public class StatementDataResponse
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
        public List<TransactionData> Transactions { get; set; }
    }

    public class TransactionData
    {
        [JsonPropertyName("transactionDate")]
        public string TransactionDate { get; set; }

        [JsonPropertyName("valueDate")]
        public string ValueDate { get; set; }

        [JsonPropertyName("debit")]
        public decimal Debit { get; set; }

        [JsonPropertyName("credit")]
        public decimal Credit { get; set; }

        [JsonPropertyName("balance")]
        public decimal Balance { get; set; }

        [JsonPropertyName("remarks")]
        public string Remarks { get; set; }
    }
}
