using System.Text.Json.Serialization;

namespace Loans.API.Entities.DTOs
{
    public class LoanInquiry
    {
        [JsonPropertyName("customerId")]
        public required string CustomerId { get; set; }

        [JsonPropertyName("schmCode")]
        public string SchemeCode { get; set; }=string.Empty;

        [JsonPropertyName("pageNum")]
        public int PageNum { get; set; } = 1;
    }
}
