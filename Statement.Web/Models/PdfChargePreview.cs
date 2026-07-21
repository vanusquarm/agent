namespace Statement.Web.Models
{
    public class PreviewChargeRequest
    {
        public string Request { get; set; }
    }

    public class PdfChargePreview
    {
        public decimal ChargeAmount { get; set; }
        public int PageCount { get; set; }
        public string Message { get; set; }
        public string TempFilePath { get; set; }
        public bool HasTransactions { get; set; }
    }
}
