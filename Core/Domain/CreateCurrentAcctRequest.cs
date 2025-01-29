using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
namespace Finacle.Core.Domain
{
    public class CreateCurrentAcctRequest
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
