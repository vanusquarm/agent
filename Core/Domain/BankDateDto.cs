using System.Text.Json.Serialization;

namespace Finacle.Core.Domain
{

    public class BankDateDto
    {
        [JsonPropertyName("bankBodDate")]
        public string BankBodDate { get; set; }
    }

}
