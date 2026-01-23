using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLPG_a.Data;
using QLPG_a.Models;

namespace QLPG_a.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: User
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users.AsNoTracking().ToListAsync();
            return View(users);
        }

        // GET: User/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id.Value);
            if (user == null) return NotFound();

            return View(user);
        }

        // GET: User/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MembershipNumber,UserName,Password,FullName,DateOfBirth,Gender,Email,Phone,Role")] User user)
        {
            if (!ModelState.IsValid) return View(user);

            // Hashing password should be implemented; for now store as-is in PasswordHash for demo
            user.PasswordHash = user.Password ?? string.Empty;

            _context.Add(user);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Thêm người dùng thành công.";
            return RedirectToAction(nameof(Index));
        }

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users.FindAsync(id.Value);
            if (user == null) return NotFound();

            return View(user);
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MembershipNumber,UserName,FullName,DateOfBirth,Gender,Email,Phone,Role")] User user)
        {
            if (id != user.Id) return NotFound();
            if (!ModelState.IsValid) return View(user);

            try
            {
                var existing = await _context.Users.FindAsync(id);
                if (existing == null) return NotFound();

                existing.MembershipNumber = user.MembershipNumber;
                existing.UserName = user.UserName;
                existing.FullName = user.FullName;
                existing.DateOfBirth = user.DateOfBirth;
                existing.Gender = user.Gender;
                existing.Email = user.Email;
                existing.Phone = user.Phone;
                existing.Role = user.Role;

                _context.Update(existing);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Cập nhật người dùng thành công.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(user.Id)) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id.Value);
            if (user == null) return NotFound();

            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Xóa người dùng thành công.";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
