using AuthSystem.DTOs;
using AuthSystem.Middleware;
using AuthSystem.Service;
using Microsoft.AspNetCore.Mvc;

namespace AuthSystem.Controllers;

public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;

    public AuthController(IUserService userService, IConfiguration configuration)
    {
        _userService = userService;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _userService.RegisterUserAsync(request);
        return result ? Ok("User registered successfully.") : BadRequest("Registration failed.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userService.AuthenticateAsync(request);
        if (user != null)
        {
            var token = JwtMiddleware.GenerateToken(user, _configuration);
            return Ok(new { Token = token });
        }
        return Unauthorized();
    }

    [HttpPost("forget-password")]
    public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordRequest request)
    {
        var result = await _userService.SendPasswordResetTokenAsync(request);
        return result ? Ok("Password reset token sent.") : BadRequest("Unable to send token.");
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var result = await _userService.ResetPasswordAsync(request);
        return result ? Ok("Password reset successful.") : BadRequest("Password reset failed.");
    }
}