using System.Text.Json.Serialization;

namespace Finacle.Core
{

public class BankDateDto
    {
        [JsonPropertyName("bankBodDate")]
        public string BankBodDate { get; set; }
    }

}
