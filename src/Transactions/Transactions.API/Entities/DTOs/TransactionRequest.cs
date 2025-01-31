using System.Text.Json.Serialization;

namespace Transactions.API.Entities.DTOs
{
    public class TransactionRequest
    {
        public string? fromDate { get; set; }
        public string? toDate { get; set; }
        public string? accountId { get; set; }
        public string? remarks { get; set; }
    }
}
