using System.Text.Json.Serialization;

namespace Branches.API.Entities.DTOs
{
    public class HolidayStatusResponse
    {
        [JsonPropertyName("holidayStatus")]
        public HolidayStatus? HolidayStatus { get; set; }
    }

    public class HolidayStatus
    {
        [JsonPropertyName("solID")]
        public string? SolID { get; set; }

        [JsonPropertyName("BranchName")]
        public string? BranchName { get; set; }

        [JsonPropertyName("HolidayStatus")]
        public string? Status { get; set; }

        [JsonPropertyName("NextWorkingDay")]
        public string? NextWorkingDay { get; set; }

        [JsonPropertyName("NextDataCentreWorkingDate")]
        public string? NextDataCentreWorkingDate { get; set; }
    }
}
