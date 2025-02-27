namespace Customers.API.Entities.DTOs
{
    public class StandingInstructionsDto
    {
    }

    public class GetStandingInstructionsRequest
    {
        public string cifId { get; set; }
    }

    public class GetStandingIstructionsResponse
    {
        public Sidetail[] SIDetails { get; set; }
        public string errorFlg { get; set; }
    }

    public class Sidetail
    {
        public string requestDate { get; set; }
        public string debitAccount { get; set; }
        public string beneficiaryAccount { get; set; }
        public string benAccountName { get; set; }
        public string frequency { get; set; }
        public string amount { get; set; }
        public string currency { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string lastProcessedDate { get; set; }
        public string nextProcessingDate { get; set; }
        public string seqNumber { get; set; }
        public string description { get; set; }
        public string priority { get; set; }
        public string status { get; set; }
        public string schmCode { get; set; }
        public string benschmCode { get; set; }
    }

    public class PostCustomerStandingInstructionsRequest
    {
        public string standingInstructionType { get; set; }
        public string customerReference { get; set; }
        public string branchReference { get; set; }
        public int standingInstructionPriority { get; set; }
        public Standinginstructionfrequency standingInstructionFrequency { get; set; }
        public string executionTime { get; set; }
        public string nextExecutionFromDate { get; set; }
        public string nextExecutionToDate { get; set; }
        public Fixedinstructiondetail[] fixedInstructionDetails { get; set; }
    }

    public class Standinginstructionfrequency
    {
        public string standingInstructionFrequencyType { get; set; }
        public string holidayStatus { get; set; }
    }

    public class Fixedinstructiondetail
    {
        public string currency { get; set; }
        public string transactionType { get; set; }
        public string accountIdentification { get; set; }
        public string amountType { get; set; }
        public string referenceNumber { get; set; }
        public string particulars { get; set; }
        public string remarks { get; set; }
        public string collectFees { get; set; }
        public float transactionAmount { get; set; }
        public int ceilingAmount { get; set; }
        public int cumulativeAmount { get; set; }
    }

    public class PostCustomerStandingInstructionsResponse
    {
        public string standingInstructionSerialNumber { get; set; }
    }
}
