using System;
using System.Collections.Generic;
using System.Text;

namespace Loans.API.Entities.DTOs
{
    public class CreateUnsecuredLoanRequest
    {
        public string AccountIdentification { get; set; }
        public string BranchReference { get; set; }
        public string ProductCode { get; set; }
        public string GlSubHeadCode { get; set; }
        public string CustomerReference { get; set; }
        public string AccountOpeningDate { get; set; }
        public decimal LoanAmount { get; set; }
        public string AccountCurrency { get; set; }
        public int LoanTermMonths { get; set; }
        public string OperAccountId { get; set; }
        public string InstallmentStartDate { get; set; }
        public string InstallmentFrequencyType { get; set; }
        public int InstallmentFrequencyStartDay { get; set; }
        public int NumberOfInstallments { get; set; }
    }

}
