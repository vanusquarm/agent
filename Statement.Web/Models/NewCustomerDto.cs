namespace Statement.Web.Models
{
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
        public bool Daily { get; set; }
        public bool Weekly { get; set; }
        public bool Monthly { get; set; }

        public short? DailyOpt { get; set; } = -1;
        public short? WeeklyOpt { get; set; } = -1;
        public short? MonthlyOpt { get; set; } = -1;
        public bool Continuous { get; set; }
        public int? ContinuousOpt { get; set; }
        public int? ContinuousPerMonth { get; set; }
    }
}
