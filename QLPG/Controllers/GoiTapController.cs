using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using QLPG_a.Models;
using QLPG_a.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace QLPG_a.Controllers
{
    [Authorize(Roles = "Admin")]
    public class GoiTapController : Controller
    {
        private readonly ICrudService<GoiTap> _crudService;

        public GoiTapController(ICrudService<GoiTap> crudService)
        {
            _crudService = crudService;
        }

        // GET: GoiTap?sort=code|name|duration|price&dir=asc|desc&q=...
        public async Task<IActionResult> Index(string? q = null, string sort = "name", string dir = "asc")
        {
            ViewData["Title"] = "Danh sách gói tập";
            ViewData["Query"] = q;
            ViewData["Sort"] = sort;
            ViewData["Dir"] = dir;

            var result = await _crudService.GetAllAsync();
            if (!result.Succeeded || result.Value == null)
            {
                TempData["Error"] = result.ErrorMessage ?? "Không thể tải danh sách gói tập.";
                return View(Enumerable.Empty<GoiTap>());
            }

            var list = result.Value.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim();
                list = list.Where(g =>
                    (!string.IsNullOrWhiteSpace(g.MaGoiTap) && g.MaGoiTap.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrWhiteSpace(g.TenGoi) && g.TenGoi.Contains(term, StringComparison.OrdinalIgnoreCase)));
            }

            list = (sort, dir) switch
            {
                ("code", "desc") => list.OrderByDescending(g => g.MaGoiTap),
                ("code", "asc") => list.OrderBy(g => g.MaGoiTap),
                ("duration", "desc") => list.OrderByDescending(g => g.ThoiHan),
                ("duration", "asc") => list.OrderBy(g => g.ThoiHan),
                ("price", "desc") => list.OrderByDescending(g => g.Gia),
                ("price", "asc") => list.OrderBy(g => g.Gia),
                ("name", "desc") => list.OrderByDescending(g => g.TenGoi),
                _ => list.OrderBy(g => g.TenGoi)
            };

            return View(list.ToList());
        }

        // GET: GoiTap/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var result = await _crudService.GetByIdAsync(id.Value);
            if (!result.Succeeded || result.Value == null) return NotFound();
            return View(result.Value);
        }

        // GET: GoiTap/Create
        public IActionResult Create() => View();

        // POST: GoiTap/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaGoiTap,TenGoi,ThoiHan,Gia,MoTa")] GoiTap goiTap)
        {
            if (!ModelState.IsValid) return View(goiTap);
            var result = await _crudService.CreateAsync(goiTap);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Không thể tạo gói tập.");
                return View(goiTap);
            }
            TempData["Success"] = "Thêm gói tập thành công.";
            return RedirectToAction(nameof(Index));
        }

        // GET: GoiTap/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var result = await _crudService.GetByIdAsync(id.Value);
            if (!result.Succeeded || result.Value == null) return NotFound();
            return View(result.Value);
        }

        // POST: GoiTap/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MaGoiTap,TenGoi,ThoiHan,Gia,MoTa")] GoiTap goiTap)
        {
            if (id != goiTap.Id) return NotFound();
            if (!ModelState.IsValid) return View(goiTap);
            var result = await _crudService.UpdateAsync(goiTap);
            if (!result.Succeeded)
            {
                if (result.ErrorCode == ServiceErrorCode.NotFound) return NotFound();
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Không thể cập nhật gói tập.");
                return View(goiTap);
            }

            TempData["Success"] = "Cập nhật gói tập thành công.";
            return RedirectToAction(nameof(Index));
        }

        // GET: GoiTap/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var result = await _crudService.GetByIdAsync(id.Value);
            if (!result.Succeeded || result.Value == null) return NotFound();
            return View(result.Value);
        }

        // POST: GoiTap/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _crudService.DeleteAsync(id);
            if (!result.Succeeded && result.ErrorCode == ServiceErrorCode.NotFound) return NotFound();
            if (result.Succeeded) TempData["Success"] = "Xóa gói tập thành công.";
            return RedirectToAction(nameof(Index));
        }
    }
}
