using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QLPG_a.Models;
using QLPG_a.Models.ViewModels;
using QLPG_a.Services;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using QLPG_a.Data;
using Microsoft.AspNetCore.Identity;

namespace QLPG_a.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AccountController> _logger;
        private readonly IApplicationDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IEmailService _emailService;

        public AccountController(IAuthService authService, ILogger<AccountController> logger,
            IApplicationDbContext context, IPasswordHasher<User> passwordHasher, IEmailService emailService)
        {
            _authService = authService;
            _logger = logger;
            _context = context;
            _passwordHasher = passwordHasher;
            _emailService = emailService;
        }

        [AllowAnonymous]
        public IActionResult Register() => View();

        [AllowAnonymous]
        public IActionResult Login() => View();

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _authService.RegisterAsync(model);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Đăng ký thất bại.");
                return View(model);
            }

            TempData["Success"] = "Đăng ký thành công! Vui lòng đăng nhập.";
            return RedirectToAction(nameof(Login));
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var loginResult = await _authService.ValidateLoginAsync(model);
            if (!loginResult.Succeeded || loginResult.Value == null)
            {
                ModelState.AddModelError(string.Empty, loginResult.ErrorMessage ?? "Sai tài khoản hoặc mật khẩu.");
                return View(model);
            }

            var user = loginResult.Value;
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim("UserName", user.UserName),
                new Claim(ClaimTypes.Role, user.Role)
            };
            await HttpContext.SignInAsync("MyCookie",
                new ClaimsPrincipal(new ClaimsIdentity(claims, "MyCookie")));

            if (user.Role == "Admin")
                return RedirectToAction("Dashboard", "Admin");

            return RedirectToAction("MemberDashboard", "Home");
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookie");
            return RedirectToAction("Login");
        }

        // ─── Forgot Password ─────────────────────────────────────────────────────

        [AllowAnonymous]
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                ModelState.AddModelError(string.Empty, "Vui lòng nhập email.");
                return View();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user != null)
            {
                user.PasswordResetToken = Guid.NewGuid().ToString("N");
                user.PasswordResetExpiry = DateTime.UtcNow.AddMinutes(30);
                await _context.SaveChangesAsync();

                var resetLink = Url.Action("ResetPassword", "Account",
                    new { token = user.PasswordResetToken }, Request.Scheme);
                await _emailService.SendPasswordResetAsync(email, user.FullName, resetLink!);
            }

            // Always show success to prevent email enumeration
            TempData["Success"] = "Nếu email tồn tại, chúng tôi đã gửi liên kết đặt lại mật khẩu.";
            return RedirectToAction(nameof(Login));
        }

        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return RedirectToAction(nameof(Login));
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.PasswordResetToken == token && u.PasswordResetExpiry > DateTime.UtcNow);
            if (user == null)
            {
                TempData["Error"] = "Liên kết đặt lại mật khẩu không hợp lệ hoặc đã hết hạn.";
                return RedirectToAction(nameof(Login));
            }
            return View(new ResetPasswordViewModel { Token = token });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.PasswordResetToken == model.Token && u.PasswordResetExpiry > DateTime.UtcNow);
            if (user == null)
            {
                TempData["Error"] = "Liên kết đặt lại mật khẩu không hợp lệ hoặc đã hết hạn.";
                return RedirectToAction(nameof(Login));
            }

            user.PasswordHash = _passwordHasher.HashPassword(user, model.NewPassword);
            user.PasswordResetToken = null;
            user.PasswordResetExpiry = null;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đặt lại mật khẩu thành công! Vui lòng đăng nhập.";
            return RedirectToAction(nameof(Login));
        }
    }
}
