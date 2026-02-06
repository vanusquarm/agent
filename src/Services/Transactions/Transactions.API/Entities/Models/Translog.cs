namespace Transactions.API.Entities.Models
{
    public partial class Translog
    {
        public Guid TransLogId { get; set; }
        public string? SourceAccount { get; set; }
        public string? DestinationAccount { get; set; }
        public string? Currency { get; set; }
        public decimal? Amount { get; set; }
        public decimal? PrevBal { get; set; }
        public decimal? PostBal { get; set; }
        public string? PayerName { get; set; }
        public string? PayeeName { get; set; }
        public string? Remarks { get; set; }
        public string? Status { get; set; }
        public DateTime? RequestTime { get; set; }
        public DateTime? EntryDate { get; set; }
        public DateTime? ResponseTime { get; set; }
        public string? ChannelId { get; set; }
        public string? ChannelReference { get; set; }
        public string? Message { get; set; }
        public string? CallbackUrl { get; set; }
        public string? CallbackStatus { get; set; }
        public DateTime? LastUpdate { get; set; }
        public bool? ChargeCustomer { get; set; }
        public string? ServiceChargeTransactionId { get; set; }
        public string? ServiceChargeEventId { get; set; }
        public decimal? ServiceCharge { get; set; }
        public bool IsReversed { get; set; }
    }

}
