using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QLPG_a.Data;
using QLPG_a.Models;
using QLPG_a.Models.ViewModels;
using System.Security.Claims;

namespace QLPG_a.Controllers
{
    // Xác thực controller
    [Authorize]
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // AllowAnonymous là cho phép truy cập cho người dùng không xác định
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }
        // Xử lý phương thức đăng ký
        [HttpPost]
        [AllowAnonymous]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var hasher = new PasswordHasher<User>();

            var user = new User
            {
                UserName = model.UserName,
                Role = "User"  // Mặc định là User
            };
            if (_context.Users.Any(u => u.UserName == model.UserName))
            {
                ModelState.AddModelError("UserName", "Tài khoản đã tồn tại");
                return View(model);
            }

            user.PasswordHash = hasher.HashPassword(user, model.Password);

            _context.Users.Add(user);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }

        [HttpPost]
        [AllowAnonymous] // Cho phép truy cập khi chưa đăng nhập
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            // Validate dữ liệu form
            if (!ModelState.IsValid)
                return View(model);

            // Tìm user theo username
            var user = _context.Users
                .FirstOrDefault(u => u.UserName == model.UserName);

            if (user == null)
            {
                ModelState.AddModelError("", "Sai tài khoản");
                return View(model);
            }

            // So sánh mật khẩu (hash)
            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(
                user, user.PasswordHash, model.Password);

            if (result != PasswordVerificationResult.Success)
            {
                ModelState.AddModelError("", "Sai mật khẩu");
                return View(model);
            }

            // Tạo claims để lưu vào cookie xác thực
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.Role, user.Role)
    };

            // Scheme phải trùng với cấu hình Authentication
            var identity = new ClaimsIdentity(claims, "MyCookie");
            var principal = new ClaimsPrincipal(identity);

            // Đăng nhập: ghi cookie
            await HttpContext.SignInAsync("MyCookie", principal);

            // Điều hướng theo role
            if (user.Role == "Admin")
                return RedirectToAction("Index", "Admin");

            return RedirectToAction("Index", "Home");
        }
        // Đăng xuất
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookie");
            return RedirectToAction("Login");
        }
    }
}