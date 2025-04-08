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
        public string AccountId { get; set; }
        public decimal TranAmt { get; set; }
        public string EventId { get; set; }
        public string EventType { get; set; }
    }

}
