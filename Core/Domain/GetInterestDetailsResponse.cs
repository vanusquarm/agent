using System;
using System.Collections.Generic;
using System.Text;

namespace Finacle.Core.Domain
{
    public class GetInterestDetailsResponseDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
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

    public class GetInterestDetailsResponse
    {
        public List<GetInterestDetailsResponseDto> InterestDetails { get; set; }
    }

}
