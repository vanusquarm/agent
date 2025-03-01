using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Acccounts.API.Entities.DTOs
{

    public class LienResponseDto
    {
        [JsonPropertyName("lienDetails")]
        public List<LienDetailDto> LienDetails { get; set; }
    }
    public class LienCreationDto
    {
        [JsonPropertyName("lienIdentification")]
        public string LienIdentification { get; set; }
    }
    public class LienDetailDto
    {
        [JsonProperty("lienIdentification")]
        public string LienIdentification { get; set; }

        [JsonProperty("lienClassification")]
        public string LienClassification { get; set; }

        [JsonProperty("lienType")]
        public string LienType { get; set; }

        [JsonProperty("lienProcessingModule")]
        public string LienProcessingModule { get; set; }

        [JsonProperty("lienStartDate")]
        public string LienStartDate { get; set; }

        [JsonProperty("lienExpiryDate")]
        public string LienExpiryDate { get; set; }

        [JsonProperty("lienPurposeReference")]
        public string LienPurposeReference { get; set; }

        [JsonProperty("lienRemarks")]
        public string LienRemarks { get; set; }

        [JsonProperty("lienRequestedDepartment")]
        public string LienRequestedDepartment { get; set; }

        [JsonProperty("lienOriginatorReference")]
        public string LienOriginatorReference { get; set; }

        [JsonProperty("newLienAmount")]
        public decimal NewLienAmount { get; set; }

        [JsonProperty("oldLienAmount")]
        public decimal OldLienAmount { get; set; }
    }
}
