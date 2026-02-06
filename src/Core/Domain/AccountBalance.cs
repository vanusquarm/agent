using System.Text.Json.Serialization;

namespace Finacle.Core.Domain
{
    public class AccountBalance
    {
        [JsonPropertyName("openingBalance")]
        public string OpeningBalance { get; set; }

        [JsonPropertyName("closingBalance")]
        public string ClosingBalance { get; set; }

        [JsonPropertyName("acctName")]
        public string AcctName { get; set; }

        [JsonPropertyName("errorFlg")]
        public string ErrorFlg { get; set; }
    }
}
