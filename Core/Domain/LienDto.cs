using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Finacle.Core.Domain
{
    public class LienRequest
    {

        [JsonPropertyName("lienType")]
        public  string LienType { get; set; }

        [JsonPropertyName("lienAmount")]
        public decimal LienAmount { get; set; }

        [JsonPropertyName("lienStartDate")]
        public  string LienStartDate { get; set; }

        [JsonPropertyName("lienExpiryDate")]
        public  string LienExpiryDate { get; set; }

        [JsonPropertyName("lienPurposeReference")]
        public  string LienPurposeReference { get; set; }

        [JsonPropertyName("lienRemarks")]
        public string LienRemarks { get; set; } = string.Empty;

        [JsonPropertyName("lienRequestedDepartment")]
        public string LienRequestedDepartment { get; set; } = string.Empty;

        [JsonPropertyName("lienOriginatorReference")]
        public string LienOriginatorReference { get; set; }= string.Empty;
    }

    public class LienResponse
    {
        [JsonPropertyName("lienIdentification")]
        public string LienIdentification { get; set; }
    }
}
