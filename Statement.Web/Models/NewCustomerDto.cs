namespace Statement.Web.Models
{
    public class ExistingCustomerInfo
    {
        public int ID { get; set; }
        public string CustomerNumber { get; set; }
        public string Branch { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
    }
    public class NewCustomerDto
    {
        public string Branch { get; set; }
        public string CustomerNumber { get; set; }
        public string CIF_ID { get; set; }
        public int ID { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string Createdby { get; set; }
        public string DefaultExt { get; set; }
        public string ExportFile { get; set; }
        public string ReportFile { get; set; }
        public int ExportFormat { get; set; }
        public string Status { get; set; }
        public DateTime? datecreated { get; set; }
        public DateTime? lastupdateddate { get; set; }
        public string lastupdatedby { get; set; }
        public bool UseExportFile { get; set; }
        public bool Allaccounts { get; set; }
        public int RegenerateDailyStmt { get; set; }
        public Frequency Frequency { get; set; } = new Frequency();
        public List<Account> Accounts { get; set; }
    }
    public class Frequency
    {
        // These map to BIT columns in database
        public bool Daily { get; set; }
        public bool Weekly { get; set; }
        public bool Monthly { get; set; }
        public bool Continuous { get; set; }


        public int? DailyOpt { get; set; } = -1;
        public int? WeeklyOpt { get; set; } = -1;
        public int? MonthlyOpt { get; set; } = -1;
        public int? ContinuousOpt { get; set; } = -1;

        // This maps to INT column in database
        public int? ContinuousPerMonth { get; set; }
    }
}
