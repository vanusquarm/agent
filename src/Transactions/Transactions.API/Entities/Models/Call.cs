namespace Transactions.API.Entities.Models
{
    public partial class Call
    {
        public int Id { get; set; }
        public string? Channel { get; set; }
        public string? RequestParam { get; set; }
        public string? IpAddress { get; set; }
        public string? Controller { get; set; }
        public string? EndPoint { get; set; }
        public DateTime? RequestDate { get; set; }
        public string? ResponseCode { get; set; }
        public string? ResponseParam { get; set; }
        public DateTime? ReponseTime { get; set; }
        public string RequestId { get; set; } = null!;
        public string? Apiservice { get; set; }
        public decimal? Amount { get; set; }
        public string? UserAgent { get; set; }
        public string? Userid { get; set; }
        public string? TraceId { get; set; }
    }
}
