namespace AuthSystem.Models;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = "User";
    public string? ResetToken { get; set; }
    public string? IPAddress { get; set; }
    public DateTime? ResetTokenExpiry { get; set; }
}