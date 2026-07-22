using System.Text.Json.Serialization;

namespace Statement.Web.Models
{
    public class StatementRecord
    {
        //new query -- custom.get_account_statement_pipelined view
        [JsonPropertyName("transactionValueDate")]
        public string? TransactionValueDate { get; set; }
        [JsonPropertyName("balance")]
        public decimal? Balance { get; set; }
        [JsonPropertyName("transactionDate")]
        public string? TransactionDate { get; set; }
        [JsonPropertyName("Debit")]
        public decimal? Debit { get; set; }
        [JsonPropertyName("Credit")]
        public decimal? Credit { get; set; }
        [JsonPropertyName("Remarks")]
        public string? Remarks { get; set; }


        //[JsonPropertyName("transactionAmount")]
        //public decimal? TransactionAmount { get; set; }
        //[JsonPropertyName("transactionReference")]
        //public string? TransactionReference { get; set; }
        //[JsonPropertyName("transactionType")]
        //public string? TransactionType { get; set; }
        //[JsonPropertyName("tranParticulars")]
        //public string? TranParticulars { get; set; }
        //[JsonPropertyName("PSTD_DATE")]
        //public string? PSTD_DATE { get; set; }
    }
}
