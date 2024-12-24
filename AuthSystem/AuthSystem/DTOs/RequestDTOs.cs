namespace AuthSystem.DTOs;

public record RegisterRequest(string Username, string Email, string Password);
public record LoginRequest(string Username, string Password);
public record ForgetPasswordRequest(string Email);
public record ResetPasswordRequest(string Token, string NewPassword);