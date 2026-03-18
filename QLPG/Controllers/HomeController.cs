using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLPG_a.Models;
using QLPG_a.Models.ViewModels;
using QLPG_a.Services;
using System.Diagnostics;

namespace QLPG_a.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHomeService _homeService;

        public HomeController(ILogger<HomeController> logger, IHomeService homeService)
        {
            _logger = logger;
            _homeService = homeService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Trang chủ";

            var result = await _homeService.GetHomeSummaryAsync();
            if (!result.Succeeded || result.Value == null)
            {
                TempData["Error"] = result.ErrorMessage ?? "Không thể tải dữ liệu trang chủ.";
                return View(Array.Empty<ThongBao>());
            }

            ViewBag.TotalMembers = result.Value.TotalMembers;
            ViewBag.ActiveMembers = result.Value.ActiveMembers;

            return View(result.Value.LatestNotices);
        }

        // GET: /Home/MemberDashboard  — MEMBER ONLY
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> MemberDashboard()
        {
            ViewData["Title"] = "Dashboard Hội Viên";
            var userName = User.Identity?.Name ?? string.Empty;

            var result = await _homeService.GetMemberDashboardAsync(userName);
            if (!result.Succeeded || result.Value == null)
            {
                TempData["Error"] = result.ErrorMessage ?? "Không thể tải thông tin hội viên.";
                return View(new MemberDashboardViewModel());
            }

            return View(result.Value);
        }

        public async Task<IActionResult> Members(string status = "all")
        {
            ViewData["Title"] = "Danh sách hội viên";
            ViewData["FilterStatus"] = status;

            var result = await _homeService.GetMembersAsync(status);
            if (!result.Succeeded || result.Value == null)
            {
                TempData["Error"] = result.ErrorMessage ?? "Không thể tải danh sách hội viên.";
                return View(Array.Empty<Member>());
            }

            return View(result.Value);
        }

        public async Task<IActionResult> DangKiGois()
        {
            ViewData["Title"] = "Các gói tập";
            var result = await _homeService.GetGoiTapsAsync();
            if (!result.Succeeded || result.Value == null)
            {
                TempData["Error"] = result.ErrorMessage ?? "Không thể tải gói tập.";
                return View(Array.Empty<GoiTap>());
            }
            return View(result.Value);
        }

        public async Task<IActionResult> TinTuc()
        {
            ViewData["Title"] = "Thông báo";
            var result = await _homeService.GetThongBaosAsync();
            if (!result.Succeeded || result.Value == null)
            {
                TempData["Error"] = result.ErrorMessage ?? "Không thể tải thông báo.";
                return View(Array.Empty<ThongBao>());
            }
            return View(result.Value);
        }

        public IActionResult LienHe()
        {
            ViewData["Title"] = "Liên hệ";
            return View();
        }

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

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
