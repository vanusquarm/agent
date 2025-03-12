using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Customers.API.Entities.DTOs
{
 

public class RelationshipManagerResponseDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("relationshipManagerId")]
        public string RelationshipManagerId { get; set; }

        [JsonPropertyName("relationshipManagerName")]
        public string RelationshipManagerName { get; set; }

        [JsonPropertyName("primaryRelationshipManager")]
        public string PrimaryRelationshipManager { get; set; }

        [JsonPropertyName("department")]
        public string Department { get; set; }

        [JsonPropertyName("pcCode")]
        public string PcCode { get; set; }

        [JsonPropertyName("pcName")]
        public string PcName { get; set; }

        [JsonPropertyName("pcEmail")]
        public string PcEmail { get; set; }

        [JsonPropertyName("pcPhNumber")]
        public string PcPhNumber { get; set; }

        [JsonPropertyName("teamName")]
        public string TeamName { get; set; }

        [JsonPropertyName("teamEmail")]
        public string TeamEmail { get; set; }
    }

}
