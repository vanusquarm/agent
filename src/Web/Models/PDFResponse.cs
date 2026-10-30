namespace Statement.Web.Models
{
    public class PDFResponse
    {
        public string? FilePath { get; set; }
        public string? Message { get; set; }
        public string? Amount { get; set; }
        public string? EmailAddress { get; set; }
        public string? CustomerName { get; set; }

        public bool HasTransactions { get; set; }
        public bool Success { get; set; }

        public int? PageCount { get; set; }
        public int? AccountCount { get; set; }
    }
    public class PDFRequest{
        public List<AccountInfo> Accounts { get; set; }
        //public List<CustomerInfo> Accounts { get; set; } = new List<CustomerInfo>();
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool isCompliance { get; set; }
        public string TypeOfSending { get; set; } = string.Empty;
        public string AccountToDebit { get; set; } = string.Empty;
        public string DebitCurrency { get; set; } = string.Empty;
        public string CIF_ID { get; set; } = string.Empty;
    }
}
