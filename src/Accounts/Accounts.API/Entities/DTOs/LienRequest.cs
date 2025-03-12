using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Acccounts.API.Entities.DTOs
{
    public class LienRequestDto
    {

        [JsonProperty("lienType")]
        public required string LienType { get; set; }

        [JsonPropertyName("lienAmount")]
        public required decimal LienAmount { get; set; }

        [JsonPropertyName("lienStartDate")]
        public required string LienStartDate { get; set; }

        [JsonPropertyName("lienExpiryDate")]
        public required string LienExpiryDate { get; set; }

        [JsonPropertyName("lienPurposeReference")]
        public required string LienPurposeReference { get; set; }

        [JsonPropertyName("lienRemarks")]
        public string LienRemarks { get; set; } = string.Empty;

        [JsonPropertyName("lienRequestedDepartment")]
        public string LienRequestedDepartment { get; set; } = string.Empty;

        [JsonPropertyName("lienOriginatorReference")]
        public string LienOriginatorReference { get; set; }= string.Empty;
    }
}
