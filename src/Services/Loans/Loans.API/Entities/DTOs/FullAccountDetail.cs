using System.Text.Json.Serialization;

namespace Loans.API.Entities.DTOs
{
public class FullAccountResponseDetailDto
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

        [JsonPropertyName("productCategory")]
        public string ProductCategory { get; set; }

        [JsonPropertyName("branchIdentifier")]
        public string BranchIdentifier { get; set; }

        [JsonPropertyName("accountOpeningDate")]
        public string AccountOpeningDate { get; set; }

        [JsonPropertyName("accountStatus")]
        public string AccountStatus { get; set; }

        [JsonPropertyName("accountState")]
        public string AccountState { get; set; }

        [JsonPropertyName("loanAmount")]
        public decimal LoanAmount { get; set; }

        [JsonPropertyName("loanTermMonths")]
        public int LoanTermMonths { get; set; }

        [JsonPropertyName("loanTermDays")]
        public int LoanTermDays { get; set; }

        [JsonPropertyName("effectiveInterestRate")]
        public decimal EffectiveInterestRate { get; set; }

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
        public decimal AccountBalance { get; set; }

        [JsonPropertyName("nextInstallmentAmount")]
        public decimal NextInstallmentAmount { get; set; }

        [JsonPropertyName("nextPaymentDate")]
        public string NextPaymentDate { get; set; }

        [JsonPropertyName("daysPastDue")]
        public int DaysPastDue { get; set; }

        [JsonPropertyName("accountClosureDate")]
        public string AccountClosureDate { get; set; }

        [JsonPropertyName("maturityDate")]
        public string MaturityDate { get; set; }
    }

}
