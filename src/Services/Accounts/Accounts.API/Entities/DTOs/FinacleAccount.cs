using System.ComponentModel.DataAnnotations;

namespace Accounts.API.Entities.DTOs
{
    public class FinacleAccount
    {
        [Display(Name = "Account Number")]
        [Required]
        [MaxLength(13, ErrorMessage = "{0} cannot exceed {1} characters.")]
        [MinLength(12, ErrorMessage = "{0} must be at least {1} characters.")]
        public string AccountNumber { get; set; }

    }
}
