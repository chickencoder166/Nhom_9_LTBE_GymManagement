using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLPG_a.Models;
using QLPG_a.Models.ViewModels;
using QLPG_a.Services;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace QLPG_a.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly ILookupService _lookupService;
        private readonly IDangKiGoiService _dangKiGoiService;

        public AdminController(IAdminService adminService, ILookupService lookupService, IDangKiGoiService dangKiGoiService)
        {
            _adminService = adminService;
            _lookupService = lookupService;
            _dangKiGoiService = dangKiGoiService;
        }

        // GET: /Admin/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            ViewData["Title"] = "Dashboard";

            var result = await _adminService.GetDashboardAsync();
            if (!result.Succeeded || result.Value == null)
            {
                TempData["Error"] = result.ErrorMessage ?? "Không thể tải Dashboard.";
                return View(new DashboardViewModel());
            }

            return View(result.Value);
        }

        // GET: /Admin/DangKiGoiTaps
        public async Task<IActionResult> DangKiGoiTaps()
        {
            ViewData["Title"] = "Quản lý gói tập";
            var result = await _adminService.GetAllGoiTapsAsync();
            if (!result.Succeeded || result.Value == null)
            {
                TempData["Error"] = result.ErrorMessage ?? "Không thể tải gói tập.";
                return View(Array.Empty<GoiTap>());
            }

            return View(result.Value);
        }

        // GET: /Admin/Members?status=all|active|expired&q=...&sort=name|status|expire&dir=asc|desc
        public async Task<IActionResult> Members(string status = "all", string? q = null,
            string sort = "name", string dir = "asc")
        {
            ViewData["Title"] = "Quản lý hội viên";
            ViewData["FilterStatus"] = status;
            ViewData["Query"] = q;
            ViewData["Sort"] = sort;
            ViewData["Dir"] = dir;

            var result = await _adminService.GetMembersAsync(status, q);
            if (!result.Succeeded || result.Value == null)
            {
                TempData["Error"] = result.ErrorMessage ?? "Không thể tải danh sách hội viên.";
                return View(Array.Empty<MemberViewModel>());
            }

            var list = result.Value.AsEnumerable();
            list = (sort, dir) switch
            {
                ("name", "desc") => list.OrderByDescending(m => m.FullName),
                ("status", "asc") => list.OrderBy(m => m.Status),
                ("status", "desc") => list.OrderByDescending(m => m.Status),
                ("expire", "asc") => list.OrderBy(m => m.EndDate ?? DateTime.MaxValue),
                ("expire", "desc") => list.OrderByDescending(m => m.EndDate ?? DateTime.MinValue),
                ("days", "asc") => list.OrderBy(m => m.RemainingDays ?? int.MaxValue),
                ("days", "desc") => list.OrderByDescending(m => m.RemainingDays ?? int.MinValue),
                _ => list.OrderBy(m => m.FullName)
            };

            return View(list.ToList());
        }

        // GET: /Admin/RegisterDangKiGoiTap
        public async Task<IActionResult> RegisterDangKiGoiTap()
        {
            ViewData["Title"] = "Đăng ký gói tập";
            ViewBag.Members = await _lookupService.GetMembersAsync();
            ViewBag.GoiTaps = await _lookupService.GetGoiTapsAsync();
            return View("RegisterSubscription");
        }

        // POST: /Admin/RegisterDangKiGoiTap
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterDangKiGoiTap([Bind("MaDangKy,MemberId,GoiTapId,NgayBatDau,TongTien")] DangKiGoi model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Members = await _lookupService.GetMembersAsync();
                ViewBag.GoiTaps = await _lookupService.GetGoiTapsAsync();
                return View("RegisterSubscription", model);
            }

            var result = await _dangKiGoiService.CreateAsync(model);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Lỗi khi lưu đăng ký.");
                ViewBag.Members = await _lookupService.GetMembersAsync();
                ViewBag.GoiTaps = await _lookupService.GetGoiTapsAsync();
                return View("RegisterSubscription", model);
            }

            TempData["Success"] = "Đăng ký gói tập cho hội viên thành công.";
            return RedirectToAction(nameof(Members));
        }

        // GET: /Admin/Reports
        public async Task<IActionResult> Reports()
        {
            ViewData["Title"] = "Thống kê";

            var result = await _adminService.GetReportsAsync();
            if (!result.Succeeded || result.Value == null)
            {
                TempData["Error"] = result.ErrorMessage ?? "Không thể tải báo cáo.";
                return View(new ReportsViewModel());
            }

            return View(result.Value);
        }

        // GET: /Admin/ExportMembersExcel
        public async Task<IActionResult> ExportMembersExcel(string status = "all", string? q = null)
        {
            var result = await _adminService.GetMembersAsync(status, q);
            var members = result.Succeeded && result.Value != null
                ? result.Value
                : Array.Empty<MemberViewModel>();

            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Hội Viên");

            // Header row
            ws.Cell(1, 1).Value = "Mã HV"; ws.Cell(1, 2).Value = "Họ Tên";
            ws.Cell(1, 3).Value = "SĐT"; ws.Cell(1, 4).Value = "Email";
            ws.Cell(1, 5).Value = "Gói hiện tại"; ws.Cell(1, 6).Value = "Ngày bắt đầu";
            ws.Cell(1, 7).Value = "Ngày hết hạn"; ws.Cell(1, 8).Value = "Trạng thái";
            ws.Cell(1, 9).Value = "Còn lại (ngày)";
            var header = ws.Range(1, 1, 1, 9);
            header.Style.Font.Bold = true;
            header.Style.Fill.BackgroundColor = XLColor.FromHtml("#dc3545");
            header.Style.Font.FontColor = XLColor.White;

            int row = 2;
            foreach (var m in members)
            {
                ws.Cell(row, 1).Value = m.MembershipNumber;
                ws.Cell(row, 2).Value = m.FullName;
                ws.Cell(row, 3).Value = m.Phone;
                ws.Cell(row, 4).Value = m.Email ?? "";
                ws.Cell(row, 5).Value = m.CurrentPackageName ?? "—";
                ws.Cell(row, 6).Value = m.StartDate?.ToString("dd/MM/yyyy") ?? "—";
                ws.Cell(row, 7).Value = m.EndDate?.ToString("dd/MM/yyyy") ?? "—";
                ws.Cell(row, 8).Value = m.Status;
                if (m.RemainingDays.HasValue)
                {
                    ws.Cell(row, 9).Value = m.RemainingDays.Value;
                }
                else
                {
                    ws.Cell(row, 9).Value = "—";
                }
                row++;
            }

            ws.Columns().AdjustToContents();

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            ms.Position = 0;
            var fileName = $"HoiVien_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            return File(ms.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        // GET: /Admin/ExportRevenueExcel
        public async Task<IActionResult> ExportRevenueExcel()
        {
            var result = await _adminService.GetReportsAsync();
            if (!result.Succeeded || result.Value == null)
                return RedirectToAction(nameof(Reports));

            var vm = result.Value;

            using var wb = new XLWorkbook();

            // Sheet 1: Revenue
            var ws1 = wb.Worksheets.Add("Doanh Thu");
            ws1.Cell(1, 1).Value = "Tháng/Năm";
            ws1.Cell(1, 2).Value = "Doanh Thu (VNĐ)";
            var h1 = ws1.Range(1, 1, 1, 2);
            h1.Style.Font.Bold = true;
            h1.Style.Fill.BackgroundColor = XLColor.FromHtml("#0d6efd");
            h1.Style.Font.FontColor = XLColor.White;
            int r = 2;
            foreach (var rev in vm.Revenues)
            {
                ws1.Cell(r, 1).Value = $"{rev.Month:00}/{rev.Year}";
                ws1.Cell(r, 2).Value = (double)rev.Total;
                ws1.Cell(r, 2).Style.NumberFormat.Format = "#,##0";
                r++;
            }
            ws1.Columns().AdjustToContents();

            // Sheet 2: Top Packages
            var ws2 = wb.Worksheets.Add("Top Gói Tập");
            ws2.Cell(1, 1).Value = "Gói Tập";
            ws2.Cell(1, 2).Value = "Số lượt đăng ký";
            ws2.Cell(1, 3).Value = "Doanh Thu (VNĐ)";
            var h2 = ws2.Range(1, 1, 1, 3);
            h2.Style.Font.Bold = true;
            h2.Style.Fill.BackgroundColor = XLColor.FromHtml("#198754");
            h2.Style.Font.FontColor = XLColor.White;
            int r2 = 2;
            foreach (var p in vm.TopPackages)
            {
                ws2.Cell(r2, 1).Value = p.PackageName;
                ws2.Cell(r2, 2).Value = p.RegistrationCount;
                ws2.Cell(r2, 3).Value = (double)p.Revenue;
                ws2.Cell(r2, 3).Style.NumberFormat.Format = "#,##0";
                r2++;
            }
            ws2.Columns().AdjustToContents();

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            ms.Position = 0;
            var fileName = $"BaoCao_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            return File(ms.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}
