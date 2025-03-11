namespace Transactions.API.Entities.DTOs
{
    public class ChargeInquiryRequest
    {
        public string accountId { get; set; }
        public decimal tranAmt { get; set; }

        public string eventId { get; set; }
        public string eventType { get; set; }
    }
}
