namespace Transactions.API.Entities.DTOs
{
    public class ChargeInquiryRequest
    {
        public string accountId { get; set; }
        public string tranAmt { get; set; }

        public string eventId { get; set; }
        public string eventType { get; set; }
    }

    public class PerformChargeInquiryDto
    {
        public required string AccountId { get; set; }
        public required decimal TranAmt { get; set; }
        public required string EventId { get; set; }
        public required string EventType { get; set; }
    }

}
