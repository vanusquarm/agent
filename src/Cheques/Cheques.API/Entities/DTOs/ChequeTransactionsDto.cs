using System.Text.Json.Serialization;

namespace Cheques.API.Entities.DTOs
{
    public class ChequeTransactionsRequest
    {
        [JsonPropertyName("chqSrlNum")]
        public required string ChqSrlNum { get; set; }

        [JsonPropertyName("tranAmount")]
        public required string TranAmount { get; set; }

        [JsonPropertyName("tranDate")]
        public required string TranDate { get; set; }
    }

    public class ChequeTransactionsResponse
    {
        [JsonPropertyName("tranDetails")]
        public List<TranDetail>? TranDetails { get; set; }
    }

    public class TranDetail
    {
        [JsonPropertyName("payeeAcctNum")]
        public string? PayeeAcctNum { get; set; }

        [JsonPropertyName("amount")]
        public string? Amount { get; set; }

        [JsonPropertyName("chqSrlnum")]
        public string? ChqSrlnum { get; set; }

        [JsonPropertyName("date")]
        public string? Date { get; set; }

        [JsonPropertyName("sortCode")]
        public string? SortCode { get; set; }

        [JsonPropertyName("solID")]
        public string? SolID { get; set; }

        [JsonPropertyName("tranParticular")]
        public string? TranParticular { get; set; }
    }


}