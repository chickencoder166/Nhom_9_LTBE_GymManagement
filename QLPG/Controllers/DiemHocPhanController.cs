using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLSV_DB.Data;
using QLSV_DB.Models;

namespace QLSV_DB.Controllers
{
    public class DiemHocPhanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DiemHocPhanController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DiemHocPhan
        public async Task<IActionResult> Index()
        {
            var list = await _context.DiemHocPhans
                .Include(d => d.SinhVien)
                .Include(d => d.HocPhan)
                .ToListAsync();
            return View(list);
        }

        // GET: DiemHocPhan/Create
        public IActionResult Create()
        {
            var sinhVienList = _context.SinhViens.Select(s => new
            {
                Id = s.Id,
                //HienThi = s.HoVaTen + " (" + s.MaSinhVien + ")"
                HienThi = $"{s.HoVaTen} ({s.MaSinhVien})"
            }).ToList();
            var hocPhanList = _context.HocPhans.Select(s => new
            {
                Id = s.Id,
                HienThi = $"{s.TenHocPhan} ({s.MaHocPhan})"
            }).ToList();
            ViewData["HocPhanId"] = new SelectList(hocPhanList, "Id", "HienThi");
            ViewData["SinhVienId"] = new SelectList(sinhVienList, "Id", "HienThi");
            return View();
        }

        // POST: DiemHocPhan/Create
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DiemHocPhan diemHocPhan)
        {
            if (ModelState.IsValid)
            {
                diemHocPhan.TinhDiem();
                _context.Add(diemHocPhan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["HocPhanId"] = new SelectList(_context.HocPhans, "Id", "TenHocPhan", diemHocPhan.HocPhanId);
            ViewData["SinhVienId"] = new SelectList(_context.SinhViens, "Id", "HoVaTen", diemHocPhan.SinhVienId);
            return View(diemHocPhan);
        }

        // GET: DiemHocPhan/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var diemHocPhan = await _context.DiemHocPhans.FindAsync(id);
            if (diemHocPhan == null) return NotFound();
            var sinhVienList = _context.SinhViens.Select(s => new
            {
                Id = s.Id,
                HienThi = $"{s.HoVaTen} ({s.MaSinhVien})"
            }).ToList();
            var hocPhanList = _context.HocPhans.Select(s => new
            {
                Id = s.Id,
                HienThi = $"{s.TenHocPhan} ({s.MaHocPhan})"
            }).ToList();
            ViewData["HocPhanId"] = new SelectList(hocPhanList, "Id", "HienThi", diemHocPhan.HocPhanId);
            ViewData["SinhVienId"] = new SelectList(sinhVienList, "Id", "HienThi", diemHocPhan.SinhVienId);
            return View(diemHocPhan);
        }

        // POST: DiemHocPhan/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DiemHocPhan diemHocPhan)
        {
            if (id != diemHocPhan.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    diemHocPhan.TinhDiem();
                    _context.Update(diemHocPhan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DiemHocPhanExists(diemHocPhan.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            var sinhVienList = _context.SinhViens.Select(s => new
            {
                Id = s.Id,
                HienThi = $"{s.HoVaTen} ({s.MaSinhVien})"
            }).ToList();
            var hocPhanList = _context.HocPhans.Select(s => new
            {
                Id = s.Id,
                HienThi = $"{s.TenHocPhan} ({s.MaHocPhan})"
            }).ToList();
            ViewData["HocPhanId"] = new SelectList(hocPhanList, "Id", "HienThi", diemHocPhan.HocPhanId);
            ViewData["SinhVienId"] = new SelectList(sinhVienList, "Id", "HienThi", diemHocPhan.SinhVienId);
            return View(diemHocPhan);
        }

        // GET: DiemHocPhan/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var diemHocPhan = await _context.DiemHocPhans
                .Include(d => d.HocPhan)
                .Include(d => d.SinhVien)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (diemHocPhan == null) return NotFound();

            return View(diemHocPhan);
        }

        // POST: DiemHocPhan/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var diemHocPhan = await _context.DiemHocPhans.FindAsync(id);
            if (diemHocPhan != null) _context.DiemHocPhans.Remove(diemHocPhan);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DiemHocPhanExists(int id)
        {
            return _context.DiemHocPhans.Any(e => e.Id == id);
        }
    }
}