using System.Text.Json.Serialization;

namespace Loans.API.Entities.DTOs
{
    public class EligibilityRequest
    {
        public int BraCode { get; set; }
        public string CusNum { get; set; } = string.Empty;
    }

    public class ManualQuickCreditRequest
    {
        public int BraCode { get; set; }
        public string CusNum { get; set; } = string.Empty;
        public string Account { get; set; } = string.Empty;
        public decimal SalEligible { get; set; }
        public string PmApprover { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
    }

    public class QuickCreditHRUndertakingRequest
    {
        public int BraCode { get; set; }
        public string CusNum { get; set; } = string.Empty;
        public string TellId { get; set; } = string.Empty;
        public decimal SalEligible { get; set; }
        public string PmApprover { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
        public int ValidationCode { get; set; }
    }

    public class CustomerDetail
    {
        [JsonPropertyName("bra_code")]
        public string Bra_Code { get; set; } = string.Empty;

        [JsonPropertyName("cus_num")]
        public string Cus_Num { get; set; } = string.Empty;

        [JsonPropertyName("avg_sal")]
        public decimal Avg_Sal { get; set; }

        [JsonPropertyName("next_pay_amt")]
        public decimal Next_Pay_Amt { get; set; }

        [JsonPropertyName("sal_elg_amt")]
        public decimal Sal_Elg_Amt { get; set; }

        [JsonPropertyName("cus_name")]
        public string Cus_Name { get; set; } = string.Empty;

        [JsonPropertyName("already_qualified")]
        public int Already_Qualified { get; set; }
    }

    public class EligibilityResponse
    {
        public string Code { get; set; } = string.Empty;
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public CustomerDetail? Cust_Det { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Error { get; set; }
        
        public int Already_Qualified { get; set; }
    }

    public class ManualQuickCreditResponse
    {
        public string Code { get; set; } = string.Empty;
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Message { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Reject_Reason { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Error { get; set; }
    }

    public class QuickCreditHRUndertakingResponse
    {
        public string Code { get; set; } = string.Empty;
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Message { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Reject_Reason { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Error { get; set; }
    }
}
