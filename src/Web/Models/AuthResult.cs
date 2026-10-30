namespace Statement.Web.Models
{
    public class AuthResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string BranchCode { get; set; }
        public string Email { get; set; }
        public string RoleId { get; set; }
        public string RawResponse { get; set; }
    }
}
