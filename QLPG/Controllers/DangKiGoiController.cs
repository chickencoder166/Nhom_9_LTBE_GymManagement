using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using QLPG_a.Models;
using QLPG_a.Services;
using QRCoder;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace QLPG_a.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DangKiGoiController : Controller
    {
        private readonly IDangKiGoiService _dangKiGoiService;
        private readonly ILookupService _lookupService;

        public DangKiGoiController(IDangKiGoiService dangKiGoiService, ILookupService lookupService)
        {
            _dangKiGoiService = dangKiGoiService;
            _lookupService = lookupService;
        }

        // GET: DangKiGoi
        public async Task<IActionResult> Index(string? sort = null, string dir = "asc")
        {
            ViewData["Sort"] = sort;
            ViewData["Dir"] = dir;
            var result = await _dangKiGoiService.GetAllAsync();
            if (!result.Succeeded || result.Value == null)
            {
                TempData["Error"] = result.ErrorMessage ?? "Không thể tải danh sách đăng ký.";
                return View(Enumerable.Empty<DangKiGoi>());
            }
            var list = result.Value.AsEnumerable();
            list = (sort, dir) switch
            {
                ("member", "desc") => list.OrderByDescending(d => d.Member?.FullName),
                ("member", _) => list.OrderBy(d => d.Member?.FullName),
                ("goi", "desc") => list.OrderByDescending(d => d.GoiTap?.TenGoi),
                ("goi", _) => list.OrderBy(d => d.GoiTap?.TenGoi),
                ("start", "desc") => list.OrderByDescending(d => d.NgayBatDau),
                ("start", _) => list.OrderBy(d => d.NgayBatDau),
                ("status", "desc") => list.OrderByDescending(d => d.TrangThai),
                ("status", _) => list.OrderBy(d => d.TrangThai),
                _ => list.OrderByDescending(d => d.NgayBatDau)
            };
            return View(list.ToList());
        }

        // GET: DangKiGoi/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var result = await _dangKiGoiService.GetByIdAsync(id.Value);
            if (!result.Succeeded || result.Value == null) return NotFound();

            // Generate QR code
            var dk = result.Value;
            var qrContent = $"MÃ ĐK: {dk.MaDangKy}\nHội viên: {dk.Member?.FullName}\nGói: {dk.GoiTap?.TenGoi}\nBắt đầu: {dk.NgayBatDau:dd/MM/yyyy}\nHết hạn: {dk.NgayKetThuc:dd/MM/yyyy}\nTrạng thái: {dk.TrangThai}";
            using var qrGenerator = new QRCodeGenerator();
            var qrData = qrGenerator.CreateQrCode(qrContent, QRCodeGenerator.ECCLevel.M);
            using var qrCode = new PngByteQRCode(qrData);
            var qrBytes = qrCode.GetGraphic(5);
            ViewBag.QrCodeBase64 = Convert.ToBase64String(qrBytes);

            return View(dk);
        }

        // GET: DangKiGoi/Create
        public async Task<IActionResult> Create()
        {
            ViewData["Members"] = await _lookupService.GetMembersAsync();
            ViewData["GoiTaps"] = await _lookupService.GetGoiTapsAsync();
            return View();
        }

        // POST: DangKiGoi/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaDangKy,MemberId,GoiTapId,NgayBatDau,GhiChu")] DangKiGoi dangKy)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Members"] = await _lookupService.GetMembersAsync();
                ViewData["GoiTaps"] = await _lookupService.GetGoiTapsAsync();
                return View(dangKy);
            }

            var result = await _dangKiGoiService.CreateAsync(dangKy);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Lỗi khi lưu đăng ký.");
                ViewData["Members"] = await _lookupService.GetMembersAsync();
                ViewData["GoiTaps"] = await _lookupService.GetGoiTapsAsync();
                return View(dangKy);
            }

            TempData["Success"] = "Đăng ký gói thành công.";
            return RedirectToAction(nameof(Index));
        }

        // GET: DangKiGoi/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var result = await _dangKiGoiService.GetByIdAsync(id.Value);
            if (!result.Succeeded || result.Value == null) return NotFound();
            ViewData["Members"] = await _lookupService.GetMembersAsync();
            ViewData["GoiTaps"] = await _lookupService.GetGoiTapsAsync();
            return View(result.Value);
        }

        // POST: DangKiGoi/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MaDangKy,MemberId,GoiTapId,NgayBatDau,NgayKetThuc,TongTien,TrangThai,GhiChu")] DangKiGoi dangKy)
        {
            if (id != dangKy.Id) return NotFound();
            if (!ModelState.IsValid)
            {
                ViewData["Members"] = await _lookupService.GetMembersAsync();
                ViewData["GoiTaps"] = await _lookupService.GetGoiTapsAsync();
                return View(dangKy);
            }

            var result = await _dangKiGoiService.UpdateAsync(dangKy);
            if (!result.Succeeded)
            {
                if (result.ErrorCode == ServiceErrorCode.NotFound) return NotFound();
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Lỗi khi cập nhật đăng ký.");
                ViewData["Members"] = await _lookupService.GetMembersAsync();
                ViewData["GoiTaps"] = await _lookupService.GetGoiTapsAsync();
                return View(dangKy);
            }

            TempData["Success"] = "Cập nhật đăng ký thành công.";
            return RedirectToAction(nameof(Index));
        }

        // GET: DangKiGoi/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var result = await _dangKiGoiService.GetByIdAsync(id.Value);
            if (!result.Succeeded || result.Value == null) return NotFound();
            return View(result.Value);
        }

        // POST: DangKiGoi/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _dangKiGoiService.DeleteAsync(id);
            if (!result.Succeeded && result.ErrorCode == ServiceErrorCode.NotFound) return NotFound();
            if (result.Succeeded) TempData["Success"] = "Xóa đăng ký thành công.";
            return RedirectToAction(nameof(Index));
        }
    }
}
