// using Finacle.Core;
using System.Text.Json.Serialization;

namespace Transactions.API.Entities.DTOs
{
    public class BankDateDto
    {
        [JsonPropertyName("bankBodDate")]
        public string BankBodDate { get; set; }
    }
}