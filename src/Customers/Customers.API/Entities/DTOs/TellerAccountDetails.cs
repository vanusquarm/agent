using System.Text.Json.Serialization;

namespace Customers.API.Entities.DTOs
{
    public class TellerAccountDetails
    {
        [JsonPropertyName("tellerDetails")]
        public List<TellerDetail> TellerDetails { get; set; }

        [JsonPropertyName("errorFlg")]
        public string ErrorFlg { get; set; }
    }

    public class TellerDetail
    {
        [JsonPropertyName("accountNumber")]
        public string AccountNumber { get; set; }

        [JsonPropertyName("tellerName")]
        public string TellerName { get; set; }

        [JsonPropertyName("debitBalanceLimit")]
        public string DebitBalanceLimit { get; set; }

        [JsonPropertyName("sanctionLimit")]
        public string SanctionLimit { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("userclass")]
        public string Userclass { get; set; }

        [JsonPropertyName("balance")]
        public string Balance { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("freezeType")]
        public string FreezeType { get; set; }

        [JsonPropertyName("acountStatus")]
        public string AcountStatus { get; set; }
    }

}