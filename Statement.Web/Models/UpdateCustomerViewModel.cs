namespace Statement.Web.Models
{
    public class UpdateCustomerViewModel
    {
        public int ID { get; set; }
        public string Branch { get; set; }
        public string CustomerNumber { get; set; }
        public string CIF_ID { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string DefaultExt { get; set; }
        public string Status { get; set; }
        public bool UseExportFile { get; set; }
        public bool Allaccounts { get; set; }
        public int RegenerateDailyStmt { get; set; }

        // Frequency settings
        public Frequency Frequency { get; set; }

        // Account list
        public List<Account> Accounts { get; set; }

        // Audit fields
        public string Createdby { get; set; }
        public DateTime? datecreated { get; set; }
        public string lastupdatedby { get; set; }
        public DateTime? lastupdateddate { get; set; }

        // Additional fields (hidden)
        public string ExportFile { get; set; }
        public string ReportFile { get; set; }
        public int ExportFormat { get; set; }
        public int? NoofDays { get; set; }
        public int? ContinuousPerMonth { get; set; }

        public UpdateCustomerViewModel()
        {
            Frequency = new Frequency();
            Accounts = new List<Account>();
        }
    }
}
