using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLPG_a.Data;
using QLPG_a.Models;
using System.Linq;
using System.Threading.Tasks;

namespace QLPG_a.Controllers
{
    public class DangKyGoiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DangKyGoiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        // GET: DangKyGoi
        public async Task<IActionResult> Index()
        {
            var list = await _context.DangKyGois
                .Include(d => d.User)
                .Include(d => d.Subcription)
                .AsNoTracking()
                .ToListAsync();
            return View(list);
        }

        // GET: DangKyGoi/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var reg = await _context.DangKyGois
                .Include(d => d.User)
                .Include(d => d.Subcription)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id.Value);
            if (reg == null) return NotFound();
            return View(reg);
        }

        // GET: DangKyGoi/Create
        public async Task<IActionResult> Create()
        {
            ViewData["Users"] = await _context.Users.AsNoTracking().ToListAsync();
            ViewData["Subcriptions"] = await _context.Subcriptions.AsNoTracking().ToListAsync();
            return View();
        }

        // POST: DangKyGoi/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaDangKy,UserId,SubcriptionId,NgayBatDau,GhiChu")] DangKyGoi dangKy)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Users"] = await _context.Users.AsNoTracking().ToListAsync();
                ViewData["Subcriptions"] = await _context.Subcriptions.AsNoTracking().ToListAsync();
                return View(dangKy);
            }
            // If member has an active subscription, mark it expired
            var active = await _context.DangKyGois
                .Where(d => d.UserId == dangKy.UserId && d.TrangThai == "Đang hoạt động")
                .ToListAsync();
            foreach (var a in active)
            {
                a.TrangThai = "Hết hạn";
                _context.Update(a);
            }

            var sub = await _context.Subcriptions.FindAsync(dangKy.SubcriptionId);
            if (sub != null)
            {
                // business rule: end date = start + (ThoiHan * 30 days)
                dangKy.NgayKetThuc = dangKy.NgayBatDau.AddDays(sub.ThoiHan * 30);
                dangKy.TongTien = sub.Gia;
                dangKy.TrangThai = "Đang hoạt động";
            }

            _context.DangKyGois.Add(dangKy);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Đăng ký gói thành công.";
            return RedirectToAction(nameof(Index));
        }

        // GET: DangKyGoi/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var reg = await _context.DangKyGois.FindAsync(id.Value);
            if (reg == null) return NotFound();
            ViewData["Users"] = await _context.Users.AsNoTracking().ToListAsync();
            ViewData["Subcriptions"] = await _context.Subcriptions.AsNoTracking().ToListAsync();
            return View(reg);
        }

        // POST: DangKyGoi/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MaDangKy,UserId,SubcriptionId,NgayBatDau,NgayKetThuc,TongTien,TrangThai,GhiChu")] DangKyGoi dangKy)
        {
            if (id != dangKy.Id) return NotFound();
            if (!ModelState.IsValid)
            {
                ViewData["Users"] = await _context.Users.AsNoTracking().ToListAsync();
                ViewData["Subcriptions"] = await _context.Subcriptions.AsNoTracking().ToListAsync();
                return View(dangKy);
            }

            try
            {
                var sub = await _context.Subcriptions.FindAsync(dangKy.SubcriptionId);
                if (sub != null)
                {
                    dangKy.NgayKetThuc = dangKy.NgayBatDau.AddMonths(sub.ThoiHan);
                    dangKy.TongTien = sub.Gia;
                }

                _context.Update(dangKy);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Cập nhật đăng ký thành công.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DangKyExists(dangKy.Id)) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: DangKyGoi/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var reg = await _context.DangKyGois
                .Include(d => d.User)
                .Include(d => d.Subcription)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id.Value);
            if (reg == null) return NotFound();
            return View(reg);
        }

        // POST: DangKyGoi/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reg = await _context.DangKyGois.FindAsync(id);
            if (reg != null)
            {
                _context.DangKyGois.Remove(reg);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Xóa đăng ký thành công.";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool DangKyExists(int id)
        {
            return _context.DangKyGois.Any(e => e.Id == id);
        }
    }
}
