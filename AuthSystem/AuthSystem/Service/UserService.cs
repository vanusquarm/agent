using AuthSystem.Datasource;
using AuthSystem.DTOs;
using AuthSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthSystem.Service;

public class UserService:IUserService
{
    private readonly ApplicationDBContext _context;

    public UserService(ApplicationDBContext context)
    {
        _context = context;
    }

    public async Task<bool> RegisterUserAsync(RegisterRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email)) return false;

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            Password = HashPassword(request.Password),
            Role = "User"
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<User?> AuthenticateAsync(LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
        if (user != null && VerifyPassword(request.Password, user.Password))
        {
            return user;
        }
        return null;
    }

    public async Task<bool> SendPasswordResetTokenAsync(ForgetPasswordRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null) return false;

        var token = Guid.NewGuid().ToString();
        user.ResetToken = token;
        user.ResetTokenExpiry = DateTime.UtcNow.AddHours(1);

        await _context.SaveChangesAsync();

        Console.WriteLine($"Reset token for {request.Email}: {token}");

        return true;
    }

    public async Task<bool> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.ResetToken == request.Token && u.ResetTokenExpiry > DateTime.UtcNow);
        if (user == null) return false;

        user.Password = HashPassword(request.NewPassword);
        user.ResetToken = null;
        user.ResetTokenExpiry = null;

        await _context.SaveChangesAsync();
        return true;
    }
    
    private string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    private bool VerifyPassword(string enteredPassword, string storedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(enteredPassword, storedPassword);
    }
}