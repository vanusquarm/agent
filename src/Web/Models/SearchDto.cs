using System.ComponentModel.DataAnnotations;

namespace Statement.Web.Models
{
    public class SearchDto
    {
        [Required(ErrorMessage = "Search parameter is required")]
        public string SearchParameter { get; set; }

        [Required(ErrorMessage = "Search value is required")]
        public string SearchValue { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        public DateTime EndDate { get; set; }
    }
}
