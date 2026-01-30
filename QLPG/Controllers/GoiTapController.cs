using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLPG_a.Data;
using QLPG_a.Models;
using System.Linq;
using System.Threading.Tasks;

namespace QLPG_a.Controllers
{
    public class GoiTapController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GoiTapController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        // GET: GoiTap
        public async Task<IActionResult> Index()
        {
            var list = await _context.Subcriptions.AsNoTracking().ToListAsync();
            return View(list);
        }

        // GET: GoiTap/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var item = await _context.Subcriptions.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id.Value);
            if (item == null) return NotFound();
            return View(item);
        }

        // GET: GoiTap/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: GoiTap/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaGoiTap,TenGoi,ThoiHan,Gia,MoTa")] GoiTap sub)
        {
            if (!ModelState.IsValid) return View(sub);
            _context.Subcriptions.Add(sub);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Thêm gói tập thành công.";
            return RedirectToAction(nameof(Index));
        }

        // GET: GoiTap/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var sub = await _context.Subcriptions.FindAsync(id.Value);
            if (sub == null) return NotFound();
            return View(sub);
        }

        // POST: GoiTap/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MaGoiTap,TenGoi,ThoiHan,Gia,MoTa")] GoiTap sub)
        {
            if (id != sub.Id) return NotFound();
            if (!ModelState.IsValid) return View(sub);
            try
            {
                _context.Update(sub);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Cập nhật gói tập thành công.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubExists(sub.Id)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: GoiTap/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var sub = await _context.Subcriptions.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id.Value);
            if (sub == null) return NotFound();
            return View(sub);
        }

        // POST: GoiTap/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sub = await _context.Subcriptions.FindAsync(id);
            if (sub != null)
            {
                _context.Subcriptions.Remove(sub);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Xóa gói tập thành công.";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool SubExists(int id)
        {
            return _context.Subcriptions.Any(e => e.Id == id);
        }
    }
}
