using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Finacle.Core.Domain
{
        public class AccountInquiryDetailsResponseDto
        {
            [JsonPropertyName("basicAccountDetails")]
            public List<BasicAccountDetailDto> BasicAccountDetails { get; set; }

            [JsonPropertyName("errorFlg")]
            public string ErrorFlg { get; set; }
        }

        public class BasicAccountDetailDto
        {
            [JsonPropertyName("customerReference")]
            public string CustomerReference { get; set; }

            [JsonPropertyName("accountIdentification")]
            public string AccountIdentification { get; set; }

            [JsonPropertyName("accountName")]
            public string AccountName { get; set; }

            [JsonPropertyName("accountNickName")]
            public string AccountNickName { get; set; }

            [JsonPropertyName("accountCurrency")]
            public string AccountCurrency { get; set; }

            [JsonPropertyName("productCode")]
            public string ProductCode { get; set; }

            [JsonPropertyName("branchIdentifier")]
            public string BranchIdentifier { get; set; }

            [JsonPropertyName("accountOpeningDate")]
            public string AccountOpeningDate { get; set; }

            [JsonPropertyName("accountStatus")]
            public string AccountStatus { get; set; }

            [JsonPropertyName("accountState")]
            public string AccountState { get; set; }

            [JsonPropertyName("loanAmount")]
            public string LoanAmount { get; set; }

            [JsonPropertyName("loanTermMonths")]
            public string LoanTermMonths { get; set; }

            [JsonPropertyName("loanTermDays")]
            public string LoanTermDays { get; set; }

            [JsonPropertyName("effectiveInterestRate")]
            public string EffectiveInterestRate { get; set; }

            [JsonPropertyName("priorityLoan")]
            public string PriorityLoan { get; set; }

            [JsonPropertyName("repricingType")]
            public string RepricingType { get; set; }

            [JsonPropertyName("repaymentMethod")]
            public string RepaymentMethod { get; set; }

            [JsonPropertyName("equatedInstallmentLoan")]
            public string EquatedInstallmentLoan { get; set; }

            [JsonPropertyName("equatedInstallmentType")]
            public string EquatedInstallmentType { get; set; }

            [JsonPropertyName("equatedInstallmentFormula")]
            public string EquatedInstallmentFormula { get; set; }

            [JsonPropertyName("accountBalance")]
            public string AccountBalance { get; set; }

            [JsonPropertyName("nextInstallmentAmount")]
            public string NextInstallmentAmount { get; set; }

            [JsonPropertyName("nextPaymentDate")]
            public string NextPaymentDate { get; set; }

            [JsonPropertyName("daysPastDue")]
            public string DaysPastDue { get; set; }

            [JsonPropertyName("accountClosureDate")]
            public string AccountClosureDate { get; set; }

            [JsonPropertyName("maturityDate")]
            public string MaturityDate { get; set; }
        }

 }


