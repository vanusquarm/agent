using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Finacle.Core.Domain
{
    public class DisbursementRequestDTO
    {
        [JsonPropertyName("transactionValueDate")]
        public string transactionValueDate { get; set; } // Date of transaction
        [JsonPropertyName("disbursementAmount")]
        public decimal disbursementAmount { get; set; } // Total disbursement amount

        [JsonPropertyName("transactionType")]
        public string transactionType { get; set; } // e.g., "C"

        [JsonPropertyName("typeOfDisbursement")]
        public string typeOfDisbursement { get; set; } // e.g., "G"

        [JsonPropertyName("deductOverdueDemands")]


        public string deductOverdueDemands { get; set; } // e.g., "Y"

        //public List<LoanRebateAssessmentDetailDTO> LoanRebateAssessmentDetailsList { get; set; } = new List<LoanRebateAssessmentDetailDTO>();
        [JsonPropertyName("feeDetails")]

        public List<FeeDetailDTO> feeDetails { get; set; } = new List<FeeDetailDTO>();
        [JsonPropertyName("disbursementModeDetailsList")]

        public List<DisbursementModeDetailDTO> disbursementModeDetailsList { get; set; } = new List<DisbursementModeDetailDTO>();
    }

    public class LoanRebateAssessmentDetailDTO
    {
        public string RebateType { get; set; }
        public string RebateEvent { get; set; }
        public decimal UserEnteredAmount { get; set; }
    }

    public class FeeDetailDTO
    {
        [JsonPropertyName("feeType")]
        public string feeType { get; set; }
        [JsonPropertyName("feeIdentification")]
        public string feeIdentification { get; set; }
        [JsonPropertyName("collectedfeeAmount")]
        public decimal collectedfeeAmount { get; set; }
    }

    public class DisbursementModeDetailDTO
    {
        [JsonPropertyName("modeOfDisbursement")]
        public string modeOfDisbursement { get; set; } // e.g., "A"
        [JsonPropertyName("creditAccountIdentification")]
        public string creditAccountIdentification { get; set; }
        [JsonPropertyName("disbursementValueDate")]

        public string disbursementValueDate { get; set; }
        // [JsonPropertyName("paymentSystemDetails")]

        // public PaymentSystemDetailDTO? PaymentSystemDetails { get; set; } = new PaymentSystemDetailDTO();

        [JsonPropertyName("transferAmount")]

        public decimal transferAmount { get; set; }
        [JsonPropertyName("transactionRemarks")]

        public string transactionRemarks { get; set; }
    }

  

}
