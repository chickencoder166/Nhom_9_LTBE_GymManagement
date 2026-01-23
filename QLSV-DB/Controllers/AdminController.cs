using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLPG_a.Models.ViewModels;
using QLPG_a.Data;
using QLPG_a.Models;

namespace QLPG_a.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            ViewData["Title"] = "Dashboard";

            var totalUsers = await _context.Users.CountAsync();
            var totalMembers = await _context.Members.CountAsync();

            var activeSubscriptions = await _context.DangKyGois
                .Where(d => d.NgayKetThuc > DateTime.Now && (d.TrangThai == null || d.TrangThai == "Đang hoạt động"))
                .CountAsync();

            // Revenue last 6 months
            var cutoff = DateTime.Now.AddMonths(-6);
            var revenues = await _context.DangKyGois
                .Where(d => d.NgayBatDau >= cutoff)
                .GroupBy(d => new { d.NgayBatDau.Year, d.NgayBatDau.Month })
                .Select(g => new MonthlyRevenue { Year = g.Key.Year, Month = g.Key.Month, Total = g.Sum(x => x.TongTien) })
                .OrderBy(r => r.Year).ThenBy(r => r.Month)
                .ToListAsync();

            var vm = new DashboardViewModel
            {
                TotalUsers = totalUsers,
                TotalMembers = totalMembers,
                ActiveSubscriptionsCount = activeSubscriptions,
                Revenues = revenues
            };

            return View(vm);
        }

        // GET: /Admin/Subcriptions
        public async Task<IActionResult> Subcriptions()
        {
            ViewData["Title"] = "Quản lý gói tập";
            var list = await _context.Subcriptions.OrderBy(s => s.TenGoi).ToListAsync();
            return View(list);
        }

        // GET: /Admin/Members?status=all|active|expired
        public async Task<IActionResult> Members(string status = "all")
        {
            ViewData["Title"] = "Quản lý hội viên";
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

        // GET: /Admin/RegisterSubscription
        public async Task<IActionResult> RegisterSubscription()
        {
            ViewData["Title"] = "Đăng ký gói tập";
            ViewBag.Users = await _context.Users.OrderBy(u => u.FullName).ToListAsync();
            ViewBag.Subcriptions = await _context.Subcriptions.OrderBy(s => s.TenGoi).ToListAsync();
            return View();
        }

        // POST: /Admin/RegisterSubscription
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterSubscription([Bind("MaDangKy,UserId,SubcriptionId,NgayBatDau,TongTien")] DangKyGoi model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Users = await _context.Users.OrderBy(u => u.FullName).ToListAsync();
                ViewBag.Subcriptions = await _context.Subcriptions.OrderBy(s => s.TenGoi).ToListAsync();
                return View(model);
            }

            // set end date based on subcription
            var sub = await _context.Subcriptions.FindAsync(model.SubcriptionId);
            if (sub != null)
            {
                model.NgayKetThuc = model.NgayBatDau.AddMonths(sub.ThoiHan);
                model.TongTien = sub.Gia;
            }

            model.TrangThai = model.NgayKetThuc > DateTime.Now ? "Đang hoạt động" : "Hết hạn";

            _context.DangKyGois.Add(model);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đăng ký gói tập cho hội viên thành công.";
            return RedirectToAction(nameof(Members));
        }

        // GET: /Admin/Reports
        public async Task<IActionResult> Reports()
        {
            ViewData["Title"] = "Thống kê";

            // Revenue per month (last 12 months)
            var from = DateTime.Now.AddMonths(-11);
            var revenues = await _context.DangKyGois
                .Where(d => d.NgayBatDau >= from)
                .GroupBy(d => new { d.NgayBatDau.Year, d.NgayBatDau.Month })
                .Select(g => new MonthlyRevenue { Year = g.Key.Year, Month = g.Key.Month, Total = g.Sum(x => x.TongTien) })
                .OrderBy(r => r.Year).ThenBy(r => r.Month)
                .ToListAsync();

            var activeMembersCount = await _context.DangKyGois
                .Where(d => d.NgayKetThuc > DateTime.Now && (d.TrangThai == null || d.TrangThai == "Đang hoạt động"))
                .Select(d => d.UserId)
                .Distinct()
                .CountAsync();

            var vm = new ReportsViewModel
            {
                Revenues = revenues,
                ActiveMembers = activeMembersCount
            };

            return View(vm);
        }
    }
}
