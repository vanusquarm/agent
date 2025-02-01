using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Transactions.API.Entities.DTOs
{
    public class EodStatusResponse
    {
        [JsonPropertyName("firstbranchstatus")]
        public string FirstBranchStatus { get; set; }

        [JsonPropertyName("secondbranchstatus")]
        public string SecondBranchStatus { get; set; }

        [JsonPropertyName("bankstatus")]
        public string BankStatus { get; set; }

        [JsonPropertyName("errorFlg")]
        public string ErrorFlg { get; set; }
    }
}