namespace Loans.API.Entities.DTOs
{
    public class OutstandingInquiryResponseDTO
    {
        public string AccountIdentification { get; set; }
        public decimal ClearBalanceAmount { get; set; }
        public decimal TotalPrincipalBilled { get; set; }
        public decimal TotalPrincipalCollected { get; set; }
        public decimal prinicipalDue { get; set; }
        public decimal TotalInterestBilled { get; set; }
        public decimal TotalInterestCollected { get; set; }
        public decimal InterestDue { get; set; }
        public decimal TotalFees { get; set; }
        public decimal TotalFeesCollected { get; set; }
        public decimal TotalFeesDue { get; set; }
        public List<OutstandingFeeDetailDTO> OutstandingFeesDetails { get; set; } = new List<OutstandingFeeDetailDTO>();
    }

    public class OutstandingFeeDetailDTO
    {
        public string EventId { get; set; }
        public string ChargeType { get; set; }
        public decimal FeeAssessedAmount { get; set; }
        public decimal FeeCollectedAmount { get; set; }
        public decimal FeeWaivedAmount { get; set; }
        public decimal FeeDueAmount { get; set; }
    }

}
