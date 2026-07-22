// Models/SearchCustomerViewModel.cs
using System;
using System.Collections.Generic;

namespace Statement.Web.Models
{
    public class SearchCustomerViewModel
    {
        public string SearchParameter { get; set; }
        public string SearchValue { get; set; }
        public List<CustomerProfileDto> Customers { get; set; } = new List<CustomerProfileDto>();
    }

    public class CustomerProfileDto
    {
        public int ID { get; set; }
        public string CIF_ID { get; set; }
        public string CustomerNumber { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string DefaultExt { get; set; }
        public string Status { get; set; }
        public bool UseExportFile { get; set; }
        public bool AllAccounts { get; set; }
        public bool Daily { get; set; }
        public bool Weekly { get; set; }
        public bool Monthly { get; set; }
        public int? DailyOpt { get; set; }
        public int? WeeklyOpt { get; set; }
        public int? MonthlyOpt { get; set; }
        public bool Continuous { get; set; }
        public int? ContinuousOpt { get; set; }
        public int ContinuousPerMonth { get; set; }
        public int RegenerateDailyStmt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? DateCreated { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public int AccountCount { get; set; }
    }
}