namespace Statement.Web.Models
{
    public class LoginModel
    {
        public string uid { get; set; }
        public string upass { get; set; }
        public string ucode { get; set; } = "DEFAULT_CODE";
    }
}
