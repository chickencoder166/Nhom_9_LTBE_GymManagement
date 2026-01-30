using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLPG_a.Data;
using QLPG_a.Models;
using QLPG_a.Models.ViewModels;
using System.Diagnostics;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace QLPG_a.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [AllowAnonymous]
        // GET: /
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Trang chủ";

            // Basic dashboard info for public home page
            var totalMembers = await _context.Users.CountAsync();
            var activeMembers = await _context.DangKyGois
                .Where(d => d.NgayKetThuc > DateTime.Now && (d.TrangThai == null || d.TrangThai == "Đang hoạt động"))
                .Select(d => d.UserId)
                .Distinct()
                .CountAsync();

            var latestNotices = await _context.ThongBaos
                .OrderByDescending(tb => tb.NgayDang)
                .Take(5)
                .ToListAsync();

            ViewBag.TotalMembers = totalMembers;
            ViewBag.ActiveMembers = activeMembers;

            return View(latestNotices);
        }

        // GET: /Home/Members?status=all|active|expired
        public async Task<IActionResult> Members(string status = "all")
        {
            ViewData["Title"] = "Danh sách hội viên";
            ViewData["FilterStatus"] = status;

            var users = await _context.Users.AsNoTracking().ToListAsync();

            if (status == "active")
            {
                var activeUserIds = await _context.DangKyGois
                    .Where(d => d.NgayKetThuc > DateTime.Now && (d.TrangThai == null || d.TrangThai == "Đang hoạt động"))
                    .Select(d => d.UserId)
                    .Distinct()
                    .ToListAsync();

                users = users.Where(u => activeUserIds.Contains(u.Id)).ToList();
            }
            else if (status == "expired")
            {
                var expiredUserIds = await _context.DangKyGois
                    .Where(d => d.NgayKetThuc <= DateTime.Now || d.TrangThai == "Hết hạn")
                    .Select(d => d.UserId)
                    .Distinct()
                    .ToListAsync();

                users = users.Where(u => expiredUserIds.Contains(u.Id)).ToList();
            }

            return View(users);
        }

        // GET: /Home/Subcriptions
        public async Task<IActionResult> Subcriptions()
        {
            ViewData["Title"] = "Các gói tập";
            var list = await _context.Subcriptions.OrderBy(s => s.TenGoi).ToListAsync();
            return View(list);
        }

        // GET: /Home/TinTuc
        public async Task<IActionResult> TinTuc()
        {
            ViewData["Title"] = "Thông báo";
            var thongBaos = await _context.ThongBaos
                .OrderByDescending(tb => tb.NgayDang)
                .ToListAsync();
            return View(thongBaos);
        }

        // GET: /Home/LienHe
        public IActionResult LienHe()
        {
            ViewData["Title"] = "Liên hệ";
            return View();
        }

        // POST: /Home/LienHe
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LienHe(FormContact form)
        {
            ViewData["Title"] = "Liên hệ";

            if (ModelState.IsValid)
            {
                TempData["Success"] = "Gửi liên hệ thành công! Chúng tôi sẽ phản hồi trong thời gian sớm nhất.";
                return RedirectToAction("LienHe");
            }

            return View(form);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
