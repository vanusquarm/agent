using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Statement.Web.Interfaces;
using Statement.Web.Models;
using System.Security.Claims;

namespace Statement.Web.Controllers
{
    [Route("Account")]
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet("Login")]
        public IActionResult Login() => View();

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Invalid credentials" });

            var authResult = await _authService.ValidateUserAsync(model.uid, model.upass);
            if (!authResult.Success)
                return Json(new { success = false, message = "Invalid username or password" });

            // Cookie Authentication
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, authResult.UserId),
            new Claim("IsCompliance", authResult.IsCompliance.ToString())
        };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties { IsPersistent = true, ExpiresUtc = DateTime.UtcNow.AddHours(8) });

            string redirectUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.Action("Index", "Home");

            return Json(new { success = true, redirectUrl });
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
