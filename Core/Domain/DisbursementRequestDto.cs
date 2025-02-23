
using System.Collections.Generic;

namespace Finacle.Core.Domain
{
    public class DisbursementRequestDTO
    {
        public string TransactionValueDate { get; set; } // Date of transaction
        public decimal DisbursementAmount { get; set; } // Total disbursement amount
        public string TransactionType { get; set; } // e.g., "C"
        public string TypeOfDisbursement { get; set; } // e.g., "G"
        public string DeductOverdueDemands { get; set; } // e.g., "Y"

        public List<LoanRebateAssessmentDetailDTO> LoanRebateAssessmentDetailsList { get; set; } = new List<LoanRebateAssessmentDetailDTO>();
        public List<FeeDetailDTO> FeeDetails { get; set; } = new List<FeeDetailDTO>();
        public List<DisbursementModeDetailDTO> DisbursementModeDetailsList { get; set; } = new List<DisbursementModeDetailDTO>();
    }

    public class LoanRebateAssessmentDetailDTO
    {
        public string RebateType { get; set; }
        public string RebateEvent { get; set; }
        public decimal UserEnteredAmount { get; set; }
    }

    public class FeeDetailDTO
    {
        public string FeeType { get; set; }
        public string FeeIdentification { get; set; }
        public decimal CollectedFeeAmount { get; set; }
    }

    public class DisbursementModeDetailDTO
    {
        public string ModeOfDisbursement { get; set; } // e.g., "A"
        public string CreditAccountIdentification { get; set; }
        public string DisbursementValueDate { get; set; }
        public PaymentSystemDetailDTO PaymentSystemDetails { get; set; } = new PaymentSystemDetailDTO();
        public decimal TransferAmount { get; set; }
        public string TransactionRemarks { get; set; }
    }

    public class PaymentSystemDetailDTO
    {
        public string CounterpartyReference { get; set; }
        public string RespondingAccountIdentification { get; set; }
        public string RespondingPartyName { get; set; }
        public string PaymentSystemIdentification { get; set; }
        public string BankIdentifierCode { get; set; }
        public string BankCode { get; set; }
        public string BranchCode { get; set; }
        public string FreeCode1 { get; set; }
        public string FreeText1 { get; set; }
    }

   
}
