using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Statement.Web.Models;
using Statement.Web.Interfaces;

namespace Statement.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ISSOService _ssoService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(ISSOService ssoService, ILogger<AccountController> logger)
        {
            _ssoService = ssoService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = "/")
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = "/")
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Please enter both username and password" });
            }

            try
            {
                var authResult = await _ssoService.ValidateUserAsync(model.Username, model.Password);

                if (!authResult.Success)
                {
                    _logger.LogWarning("Login failed for user: {Username}", model.Username);
                    return Json(new { success = false, message = authResult.Message ?? "Invalid username or password" });
                }

                var displayName = authResult.UserName ?? authResult.UserId;
                var welcomeMessage = $"Welcome back, {displayName}!";

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, displayName),
                    new Claim(ClaimTypes.NameIdentifier, authResult.UserId),
                    new Claim(ClaimTypes.Role, authResult.RoleId ?? "User"),
                    new Claim("WelcomeMessage", welcomeMessage),
                    new Claim("FullName", displayName),
                };

                if (!string.IsNullOrEmpty(authResult.Email))
                {
                    claims.Add(new Claim(ClaimTypes.Email, authResult.Email));
                }

                if (!string.IsNullOrEmpty(authResult.BranchCode))
                {
                    claims.Add(new Claim("BranchCode", authResult.BranchCode));
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30),
                    IssuedUtc = DateTimeOffset.UtcNow,
                    AllowRefresh = true
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                HttpContext.Session.SetString("user_name", displayName);
                HttpContext.Session.SetString("user_id", authResult.UserId);
                HttpContext.Session.SetString("user_email", authResult.Email ?? "");
                HttpContext.Session.SetString("branch_code", authResult.BranchCode ?? "");
                HttpContext.Session.SetString("role_id", authResult.RoleId ?? "");
                HttpContext.Session.SetString("WelcomeMessage", welcomeMessage);

                _logger.LogInformation("User {UserId} logged in successfully", authResult.UserId);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Json(new { success = true, redirectUrl = returnUrl });
                }

                return Json(new { success = true, redirectUrl = Url.Action("Index", "Home") });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login error for user: {Username}", model.Username);
                return Json(new { success = false, message = "An error occurred during login. Please try again." });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}