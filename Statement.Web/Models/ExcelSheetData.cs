using System.Data;

namespace Statement.Web.Models
{
    public class ExcelSheetData
    {
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string AccountCurrency { get; set; }
        public string BranchName { get; set; }
        public string Address { get; set; }
        public string ProductType { get; set; }
        public DataTable Statement { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal ClosingBalance { get; set; }
        public string SheetName { get; set; }
        public bool HasTransactions { get; set; }
    }
}
