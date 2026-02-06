using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Text.Json.Serialization;

namespace Accounts.API.Entities.DTOs
{
    public class TerminalOfficeAcctRequest
    {
        [JsonPropertyName("TerminalId")]
        public string TerminalId { get; set; }
    }
}
