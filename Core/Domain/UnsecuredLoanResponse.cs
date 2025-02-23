using System;
using System.Collections.Generic;
using System.Text;

namespace Finacle.Core.Domain
{
    public class BasicAccountDetailsDto
    {
        public string CustomerReference { get; set; }
        public string AccountIdentification { get; set; }
        public string AccountName { get; set; }
        public string AccountNickName { get; set; }
        public string AccountCurrency { get; set; }
        public string ProductCode { get; set; }
        public string ProductCategory { get; set; }
        public string BranchIdentifier { get; set; }
        public DateTime AccountOpeningDate { get; set; }
        public string OperativeAccount { get; set; }
        public string AccountStatus { get; set; }
        public string AccountState { get; set; }
        public decimal LoanAmount { get; set; }
        public int LoanTermMonths { get; set; }
        public int LoanTermDays { get; set; }
        public decimal EffectiveInterestRate { get; set; }
        public string PriorityLoan { get; set; }
        public string RepricingType { get; set; }
        public string RepaymentMethod { get; set; }
        public string EquatedInstallmentLoan { get; set; }
        public string EquatedInstallmentType { get; set; }
        public string EquatedInstallmentFormula { get; set; }
        public decimal AccountBalance { get; set; }
        public decimal NextInstallmentAmt { get; set; }
        public DateTime NextPaymentDate { get; set; }
        public int DaysPastDue { get; set; }
        public DateTime AccountClosureDate { get; set; }
        public DateTime MaturityDate { get; set; }
    }

    public class LoanInstallmentDetailsDto
    {
        public string InstallmentId { get; set; }
        public DateTime InstallmentStartDate { get; set; }
        public string InstallmentFrequency { get; set; }
        public int NoOfInstallments { get; set; }
        public decimal InstallmentAmount { get; set; }
    }

    public class LoanInterestDetailsDto
    {
        public string InterestTableCode { get; set; }
        public string InterestTableVersionNumber { get; set; }
        public decimal AccountPreferentialInterestDebit { get; set; }
        public decimal CustomerPreferentialInterestDebit { get; set; }
        public decimal ChannelPreferentialInterestDebit { get; set; }
        public decimal MinimumInterestPercentDebit { get; set; }
        public decimal MaximumInterestPercentDebit { get; set; }
        public int InterestPeriodInMonths { get; set; }
        public int InterestPeriodInDays { get; set; }
        public decimal Spread { get; set; }
    }



    public class UnsecuredLoanResponseDto
    {
        public BasicAccountDetailsDto BasicAccountDetails { get; set; }
        public List<LoanInstallmentDetailsDto> LoanInstallmentDetails { get; set; }
        public List<LoanInterestDetailsDto> LoanInterestDetails { get; set; }
        public List<FeesDetailsDto> FeesDetails { get; set; }
        public List<RelatedPartyDetailsDto> RelatedPartyDetails { get; set; }
    }

}
