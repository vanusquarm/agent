using System;
using System.Collections.Generic;

namespace Finacle.Core.Domain
{
    public class FeesDetailsDto
    {
        public string EventId { get; set; }
        public string ChargeType { get; set; }
        public int MaxNoOfAssessmnts { get; set; }
        public string AssessmentFrequency { get; set; }
        public string NextAssessDate { get; set; } // Changed from DateOnly to DateTime
        public string RecoverableFlag { get; set; }
        public string DeductibleAtDisbursement { get; set; }
        public int FeeTenorMonths { get; set; }
        public string TenorEffectiveDate { get; set; } // Changed from DateOnly to DateTime
        public string AssessDmdFlg { get; set; }
        public UserEnteredAmountDto UserEnteredAmount { get; set; }
    }

    public class UserEnteredAmountDto
    {
        public decimal Value { get; set; }
        public string Currency { get; set; }
    }

    public class RelatedPartyDetailsDto
    {
        public string RelationType { get; set; }
        public string RelationCode { get; set; }
        public string CustomerReference { get; set; }
        public string AddressType { get; set; }
    }

    public class UpdateLoanFeesRequestDto
    {
        public List<FeesDetailsDto> FeesDetails { get; set; }
        public List<RelatedPartyDetailsDto> RelatedPartyDetails { get; set; }
    }

}
