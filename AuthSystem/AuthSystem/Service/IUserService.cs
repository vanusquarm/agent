using AuthSystem.DTOs;
using AuthSystem.Models;

namespace AuthSystem.Service;

public interface IUserService
{
    Task<bool> RegisterUserAsync(RegisterRequest request);
    Task<User?> AuthenticateAsync(LoginRequest request);
    Task<bool> SendPasswordResetTokenAsync(ForgetPasswordRequest request);
    Task<bool> ResetPasswordAsync(ResetPasswordRequest request);
}