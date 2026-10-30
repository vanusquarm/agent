using Statement.Web.Models;

namespace Statement.Web.Interfaces
{
    public interface ISSOService
    {
        Task<AuthResult> ValidateUserAsync(string uid, string upass);
    }
}
