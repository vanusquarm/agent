using System.ComponentModel.DataAnnotations;

namespace Accounts.API.Entities.DTOs
{
    public class BasisAccount
    {
        [Required]
        [Display(Name = "Branch Code")]
        [MaxLength(3, ErrorMessage = "{0} cannot exceed {1} characters.")]
        public string   BranchCode   { get; set; }
        [Required]
        [Display(Name = "Customer Number")]
        [MaxLength(6, ErrorMessage = "{0} cannot exceed {1} characters.")]
        public string CustomerNumber { get; set; }
        [Required]
        [Display(Name = "Currency Code")]
        [MaxLength(1, ErrorMessage = "{0} cannot exceed {1} characters.")]
        public string CurrencyCode { get; set; }
        [Required]
        [Display(Name = "Ledger Code")]
        [MaxLength(2, ErrorMessage = "{0} cannot exceed {1} characters.")]
        public string LedgerCode { get; set; }
        [Required]
        [Display(Name = "Sub Account")]
        [MaxLength(2, ErrorMessage = "{0} cannot exceed {1} characters.")]
        public string SubAccount { get; set; }

    }
}
