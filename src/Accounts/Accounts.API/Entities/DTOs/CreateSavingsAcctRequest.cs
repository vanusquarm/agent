using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Accounts.API.Entities.DTOs
{
    public class CreateSavingsAcctRequest
    {
        [JsonPropertyName("customerReference")]
        [MaxLength(50)]
        public string CustomerReference { get; set; }

        [JsonPropertyName("productCode")]
        [MaxLength(5)]
        public string ProductCode { get; set; }

        [JsonPropertyName("accountNickName")]
        [MaxLength(10)]
        public string AccountNickName { get; set; }

        [JsonPropertyName("accountCurrency")]
        [MaxLength(3)]
        public string AccountCurrency { get; set; }

        [JsonPropertyName("branchReference")]
        [MaxLength(6)]
        public string BranchReference { get; set; }

        [JsonPropertyName("modeOfOperation")]
        [MaxLength(5)]
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
