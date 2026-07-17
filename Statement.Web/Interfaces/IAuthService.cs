using Statement.Web.Models;

namespace Statement.Web.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> ValidateUserAsync(string username, string password);
    }
}
