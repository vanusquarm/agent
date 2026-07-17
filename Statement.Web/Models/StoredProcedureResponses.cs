namespace Statement.Web.Models
{
    public class StoreProcedureResponses
    {

    }
    public class TransactionLogprocedureResponse
    {
        public string Result { get; set; }
        public int Status { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
    public class BranchAccountProcedureResponse
    {
        public string DestinationAccountNumber { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
