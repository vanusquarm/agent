namespace Loans.API.Entities.DTOs
{
    public class UnsecuredLoanInterestRateDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NumberOfDays { get; set; }
        public string InterestTableCode { get; set; }
        public string VersionNumber { get; set; }
        public decimal AccountPreferentialPercentage { get; set; }
        public decimal CustomerPreferentialPercentage { get; set; }
        public decimal SpreadPercentage { get; set; }
        public decimal ChannelPreferentialPercentage { get; set; }
        public decimal NegotiatedPercentage { get; set; }
        public decimal MinimumInterestPercentage { get; set; }
        public decimal MaximumInterestPercentage { get; set; }
        public string ReviewOnSlabStartDateFlag { get; set; }
        public int InterestRatePeriodMonths { get; set; }
        public int InterestRatePeriodDays { get; set; }
        public string InterpolateForBrokenPeriodFlag { get; set; }
        public string InterpolationMethod { get; set; }
        public string InterestRateChangeReasonCode { get; set; }
    }

}
