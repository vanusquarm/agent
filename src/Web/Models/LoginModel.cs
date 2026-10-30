namespace Statement.Web.Models
{
    public class LoginModel
    {
        public string uid { get; set; }
        public string upass { get; set; }
        public string ucode { get; set; } = "DEFAULT_CODE";
    }
    public class LoginViewModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
