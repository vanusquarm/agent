using System.Text.Json.Serialization;

namespace Acccounts.API.Entities.DTOs
{
    public class CreateSavingsAcctRequest
    {
        [JsonPropertyName("customerReference")]
        public string CustomerReference { get; set; }

        [JsonPropertyName("productCode")]
        public string ProductCode { get; set; }

        [JsonPropertyName("accountNickName")]
        public string AccountNickName { get; set; }

        [JsonPropertyName("accountCurrency")]
        public string AccountCurrency { get; set; }

        [JsonPropertyName("branchReference")]
        public string BranchReference { get; set; }

        [JsonPropertyName("modeOfOperation")]
        public string ModeOfOperation { get; set; }

        [JsonPropertyName("accountOpeningDate")]
        public string AccountOpeningDate { get; set; }

        [JsonPropertyName("alertsRequired")]
        public string AlertsRequired { get; set; }

        [JsonPropertyName("alertsType")]
        public string AlertsType { get; set; }

        [JsonPropertyName("alertsFeatures")]
        public string AlertsFeatures { get; set; }
    }
}
