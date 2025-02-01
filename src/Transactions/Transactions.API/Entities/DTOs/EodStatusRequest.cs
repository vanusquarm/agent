using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Transactions.API.Entities.DTOs
{
    public class EodStatusRequest
    {
        [JsonPropertyName("firstbranchId")]
        public string FirstBranchId { get; set; }

        [JsonPropertyName("secondbranchId")]
        public string SecondBranchId { get; set; }
    }
}