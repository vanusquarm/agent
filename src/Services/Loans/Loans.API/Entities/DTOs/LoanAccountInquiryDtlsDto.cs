using System.Text.Json.Serialization;

namespace Loans.API.Entities.DTOs
{
    public class LoanAccountInquiryDtlsDto
    {
        [JsonPropertyName("customerId")]
        public string customerId;
    }
}
