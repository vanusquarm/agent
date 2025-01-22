using System.Text.Json.Serialization;

namespace Acccounts.API.Entities.DTOs
{
    public class CreateSavingsAcctRequest
    {
        [JsonPropertyName("customerReference")]
        public string CustomerReference { get; set; }

        [JsonPropertyName("productCode")]
        public string ProductCode { get; set; }

        [JsonPropertyName("glSubHeadCode")]
        public string GlSubHeadCode { get; set; }

        [JsonPropertyName("accountCurrency")]
        public string AccountCurrency { get; set; }

        [JsonPropertyName("branchReference")]
        public string BranchReference { get; set; }
    }
}
