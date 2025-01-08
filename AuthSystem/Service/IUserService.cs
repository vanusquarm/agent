using AuthSystem.DTOs;
using AuthSystem.Models;

namespace AuthSystem.Service;

public interface IUserService
{
    Task<bool> RegisterUserAsync(RegisterRequest request, string? ipAddress);
    Task<User?> AuthenticateAsync(LoginRequest request, string? ipAddress);
    Task<bool> SendPasswordResetTokenAsync(ForgetPasswordRequest request);
    Task<bool> ResetPasswordAsync(ResetPasswordRequest request);
}