namespace Statement.Web.Models
{
    public class CustomerInfo
    {
        public string CIF_ID { get; set; }
        public int BranchCode { get; set; }
        public string Branch { get; set; }
        public double Balance { get; set; }
        public string AccountNumber { get; set; }
        public string ProductType { get; set; }
        public string ProductCode { get; set; }
        public string AccountCurrency { get; set; }
        public int CustomerNo { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public string acctSolId { get; set; }
        public string? Schmcode { get; set; }
        public string ReportFile { get; set; }
        public int ExportFormat { get; set; }
        public string DefaultExt { get; set; }
        public bool UseExportFile { get; set; }
        public bool AllAccounts { get; set; }
        public bool Daily { get; set; }
        public bool Weekly { get; set; }
        public bool Monthly { get; set; }
        public int DailyOpt { get; set; }
        public int WeeklyOpt { get; set; }
        public int MonthlyOpt { get; set; }
        public string lastupdatedby { get; set; }
        public DateTime lastupdateddate { get; set; }
        public int RegenerateDailyStmt { get; set; }
    }
}
