using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using QLPG_a.Models.ViewModels;
using QLPG_a.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QLPG_a.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MemberController : Controller
    {
        private readonly IUserService _userService;

        public MemberController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index(string? q = null, string status = "all")
        {
            ViewData["Query"] = q;
            ViewData["FilterStatus"] = status;

            var result = await _userService.SearchMembersAsync(q, status);
            if (!result.Succeeded || result.Value == null)
            {
                TempData["Error"] = result.ErrorMessage ?? "Không thể tải danh sách hội viên.";
                return View(new List<MemberViewModel>());
            }

            return View(result.Value);
        }

        public IActionResult Create() => View();
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _userService.GetByIdAsync(id);
            if (!result.Succeeded || result.Value == null) return NotFound();
            return View(result.Value);
        }
        public async Task<IActionResult> Details(int id)
        {
            var result = await _userService.GetByIdAsync(id);
            if (!result.Succeeded || result.Value == null) return NotFound();
            return View(result.Value);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _userService.GetByIdAsync(id);
            if (!result.Succeeded || result.Value == null) return NotFound();
            return View(result.Value);
        }
    }
}
