using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using QLPG_a.Models;
using QLPG_a.Services;

namespace QLPG_a.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IWebHostEnvironment _environment;

        public UserController(IUserService userService, IWebHostEnvironment environment)
        {
            _userService = userService;
            _environment = environment;
        }

        // GET: User?sort=code|username|name|role&dir=asc|desc&q=...
        public async Task<IActionResult> Index(string? q = null, string sort = "name", string dir = "asc")
        {
            ViewData["Title"] = "Quản lý tài khoản";
            ViewData["Query"] = q;
            ViewData["Sort"] = sort;
            ViewData["Dir"] = dir;

            var result = await _userService.GetAllAsync();
            if (!result.Succeeded || result.Value == null)
            {
                TempData["Error"] = result.ErrorMessage ?? "Không thể tải danh sách người dùng.";
                return View(new List<User>());
            }

            var list = result.Value.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim();
                list = list.Where(u =>
                    (!string.IsNullOrWhiteSpace(u.MembershipNumber) && u.MembershipNumber.Contains(term)) ||
                    (!string.IsNullOrWhiteSpace(u.UserName) && u.UserName.Contains(term)) ||
                    (!string.IsNullOrWhiteSpace(u.FullName) && u.FullName.Contains(term)) ||
                    (!string.IsNullOrWhiteSpace(u.Phone) && u.Phone.Contains(term)) ||
                    (!string.IsNullOrWhiteSpace(u.Email) && u.Email.Contains(term)));
            }

            list = (sort, dir) switch
            {
                ("code", "desc") => list.OrderByDescending(u => u.MembershipNumber),
                ("code", "asc") => list.OrderBy(u => u.MembershipNumber),
                ("username", "desc") => list.OrderByDescending(u => u.UserName),
                ("username", "asc") => list.OrderBy(u => u.UserName),
                ("role", "desc") => list.OrderByDescending(u => u.Role),
                ("role", "asc") => list.OrderBy(u => u.Role),
                ("name", "desc") => list.OrderByDescending(u => u.FullName),
                _ => list.OrderBy(u => u.FullName)
            };

            return View(list.ToList());
        }

        // GET: User/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var result = await _userService.GetByIdAsync(id.Value);
            if (!result.Succeeded || result.Value == null) return NotFound();
            return View(result.Value);
        }

        // GET: User/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MembershipNumber,UserName,Password,FullName,DateOfBirth,Gender,Email,Phone,Role,AvatarFile")] User user)
        {
            if (!ModelState.IsValid) return View(user);

            if (!await TryStoreAvatarAsync(user))
            {
                return View(user);
            }

            if (string.IsNullOrWhiteSpace(user.Password))
            {
                ModelState.AddModelError("Password", "Vui lòng nhập mật khẩu.");
                return View(user);
            }

            var result = await _userService.CreateAsync(user, user.Password);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Không thể tạo người dùng.");
                return View(user);
            }

            TempData["Success"] = "Thêm người dùng thành công.";
            return RedirectToAction(nameof(Index));
        }

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var result = await _userService.GetByIdAsync(id.Value);
            if (!result.Succeeded || result.Value == null) return NotFound();
            return View(result.Value);
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MembershipNumber,UserName,FullName,DateOfBirth,Gender,Email,Phone,Role,AvatarFile")] User user)
        {
            if (id != user.Id) return NotFound();
            if (!ModelState.IsValid) return View(user);

            var old = await _userService.GetByIdAsync(id);
            if (old.Succeeded && old.Value != null)
            {
                user.AvatarPath = old.Value.AvatarPath;
            }

            if (!await TryStoreAvatarAsync(user))
            {
                return View(user);
            }

            var result = await _userService.UpdateAsync(user);
            if (!result.Succeeded)
            {
                if (result.ErrorCode == ServiceErrorCode.NotFound) return NotFound();
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Không thể cập nhật người dùng.");
                return View(user);
            }

            TempData["Success"] = "Cập nhật người dùng thành công.";

            return RedirectToAction(nameof(Index));
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var result = await _userService.GetByIdAsync(id.Value);
            if (!result.Succeeded || result.Value == null) return NotFound();
            return View(result.Value);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _userService.DeleteAsync(id);
            if (!result.Succeeded && result.ErrorCode == ServiceErrorCode.NotFound) return NotFound();
            if (result.Succeeded) TempData["Success"] = "Xóa người dùng thành công.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> TryStoreAvatarAsync(User user)
        {
            if (user.AvatarFile == null || user.AvatarFile.Length == 0)
            {
                return true;
            }

            var extension = Path.GetExtension(user.AvatarFile.FileName).ToLowerInvariant();
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            if (!allowed.Contains(extension))
            {
                ModelState.AddModelError("AvatarFile", "Chỉ chấp nhận ảnh JPG, PNG hoặc WEBP.");
                return false;
            }

            if (user.AvatarFile.Length > 2 * 1024 * 1024)
            {
                ModelState.AddModelError("AvatarFile", "Kích thước ảnh tối đa 2MB.");
                return false;
            }

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "avatars");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid():N}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);
            await using var stream = new FileStream(filePath, FileMode.Create);
            await user.AvatarFile.CopyToAsync(stream);

            user.AvatarPath = $"/uploads/avatars/{fileName}";
            return true;
        }
    }
}

