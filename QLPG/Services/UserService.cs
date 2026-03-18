using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QLPG_a.Data;
using QLPG_a.Models;
using QLPG_a.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLPG_a.Services
{
    public class UserService : IUserService
    {
        private readonly IApplicationDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IApplicationDbContext context,
            IPasswordHasher<User> passwordHasher,
            ILogger<UserService> logger)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        public async Task<ServiceResult<IReadOnlyList<User>>> GetAllAsync()
        {
            var users = await _context.Users.AsNoTracking().ToListAsync();
            return ServiceResult<IReadOnlyList<User>>.Success(users);
        }

        public async Task<ServiceResult<User>> GetByIdAsync(int id)
        {
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
            return user == null
                ? ServiceResult<User>.Fail(ServiceErrorCode.NotFound, "Người dùng không tồn tại.")
                : ServiceResult<User>.Success(user);
        }

        public async Task<ServiceResult<User>> CreateAsync(User user, string? plainPassword)
        {
            if (user == null)
                return ServiceResult<User>.Fail(ServiceErrorCode.Validation, "Dữ liệu người dùng không hợp lệ.");

            if (string.IsNullOrWhiteSpace(plainPassword))
                return ServiceResult<User>.Fail(ServiceErrorCode.Validation, "Mật khẩu không được để trống.");

            if (await _context.Users.AnyAsync(u => u.UserName == user.UserName))
                return ServiceResult<User>.Fail(ServiceErrorCode.Conflict, "Tên đăng nhập đã tồn tại.");

            if (!string.IsNullOrWhiteSpace(user.MembershipNumber) &&
                await _context.Users.AnyAsync(u => u.MembershipNumber == user.MembershipNumber))
                return ServiceResult<User>.Fail(ServiceErrorCode.Conflict, "Mã người dùng đã tồn tại.");

            user.Role = string.IsNullOrWhiteSpace(user.Role) ? "Member" : user.Role;
            user.PasswordHash = _passwordHasher.HashPassword(user, plainPassword);

            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return ServiceResult<User>.Success(user);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo người dùng.");
                return ServiceResult<User>.Fail(ServiceErrorCode.Unexpected, "Không thể tạo người dùng. Vui lòng thử lại.");
            }
        }

        public async Task<ServiceResult> UpdateAsync(User user)
        {
            if (user == null)
                return ServiceResult.Fail(ServiceErrorCode.Validation, "Dữ liệu người dùng không hợp lệ.");

            var existing = await _context.Users.FindAsync(user.Id);
            if (existing == null)
                return ServiceResult.Fail(ServiceErrorCode.NotFound, "Người dùng không tồn tại.");

            if (await _context.Users.AnyAsync(u => u.UserName == user.UserName && u.Id != user.Id))
                return ServiceResult.Fail(ServiceErrorCode.Conflict, "Tên đăng nhập đã tồn tại.");

            if (!string.IsNullOrWhiteSpace(user.MembershipNumber) &&
                await _context.Users.AnyAsync(u => u.MembershipNumber == user.MembershipNumber && u.Id != user.Id))
                return ServiceResult.Fail(ServiceErrorCode.Conflict, "Mã người dùng đã tồn tại.");

            existing.MembershipNumber = user.MembershipNumber;
            existing.UserName = user.UserName;
            existing.FullName = user.FullName;
            existing.DateOfBirth = user.DateOfBirth;
            existing.Gender = user.Gender;
            existing.Email = user.Email;
            existing.Phone = user.Phone;
            existing.Role = string.IsNullOrWhiteSpace(user.Role) ? existing.Role : user.Role;
            existing.AvatarPath = user.AvatarPath;

            try
            {
                _context.Users.Update(existing);
                await _context.SaveChangesAsync();
                return ServiceResult.Success();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Lỗi xung đột khi cập nhật người dùng.");
                return ServiceResult.Fail(ServiceErrorCode.Concurrency, "Dữ liệu đã bị thay đổi. Vui lòng tải lại.");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật người dùng.");
                return ServiceResult.Fail(ServiceErrorCode.Unexpected, "Không thể cập nhật người dùng. Vui lòng thử lại.");
            }
        }

        public async Task<ServiceResult> DeleteAsync(int id)
        {
            var existing = await _context.Users.FindAsync(id);
            if (existing == null)
                return ServiceResult.Fail(ServiceErrorCode.NotFound, "Người dùng không tồn tại.");

            try
            {
                _context.Users.Remove(existing);
                await _context.SaveChangesAsync();
                return ServiceResult.Success();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa người dùng.");
                return ServiceResult.Fail(ServiceErrorCode.Unexpected, "Không thể xóa người dùng. Vui lòng thử lại.");
            }
        }

        public async Task<ServiceResult<IReadOnlyList<MemberViewModel>>> SearchMembersAsync(string? query, string? status = null)
        {
            var users = _context.Users.AsNoTracking().Where(u => u.Role == "Member");
            if (!string.IsNullOrWhiteSpace(query))
            {
                users = users.Where(u => u.FullName.Contains(query) || u.UserName.Contains(query) || u.Phone.Contains(query) || u.MembershipNumber.Contains(query));
            }

            var subscriptions = await _context.DangKiGois
                .Include(d => d.GoiTap)
                .AsNoTracking()
                .OrderByDescending(d => d.NgayBatDau)
                .ToListAsync();

            var list = await users.Select(u => new MemberViewModel
            {
                Id = u.Id,
                MembershipNumber = u.MembershipNumber,
                FullName = u.FullName,
                Phone = u.Phone,
                Email = u.Email
            }).ToListAsync();

            foreach (var member in list)
            {
                var current = subscriptions.FirstOrDefault(d => d.MemberId == member.Id && d.TrangThai == "Đang hoạt động" && d.NgayKetThuc > DateTime.Now)
                             ?? subscriptions.FirstOrDefault(d => d.MemberId == member.Id);

                member.CurrentPackageName = current?.GoiTap?.TenGoi;
                member.StartDate = current?.NgayBatDau;
                member.EndDate = current?.NgayKetThuc;
                member.Status = current == null
                    ? "Chưa đăng ký"
                    : (current.NgayKetThuc > DateTime.Now && (current.TrangThai == null || current.TrangThai == "Đang hoạt động")
                        ? "Đang hoạt động"
                        : "Hết hạn");
                member.RemainingDays = current != null ? (int)Math.Ceiling((current.NgayKetThuc.Date - DateTime.Now.Date).TotalDays) : null;
            }

            if (!string.IsNullOrWhiteSpace(status) && status != "all")
            {
                list = list.Where(m => status == "active" ? m.Status == "Đang hoạt động" : m.Status == "Hết hạn").ToList();
            }

            return ServiceResult<IReadOnlyList<MemberViewModel>>.Success(list);
        }

        public async Task<ServiceResult<bool>> UserNameExistsAsync(string userName, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return ServiceResult<bool>.Fail(ServiceErrorCode.Validation, "Tên đăng nhập không hợp lệ.");

            var exists = excludeId.HasValue
                ? await _context.Users.AnyAsync(u => u.UserName == userName && u.Id != excludeId.Value)
                : await _context.Users.AnyAsync(u => u.UserName == userName);
            return ServiceResult<bool>.Success(exists);
        }

        public async Task<ServiceResult<bool>> MembershipNumberExistsAsync(string membershipNumber, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(membershipNumber))
                return ServiceResult<bool>.Fail(ServiceErrorCode.Validation, "Mã người dùng không hợp lệ.");

            var exists = excludeId.HasValue
                ? await _context.Users.AnyAsync(u => u.MembershipNumber == membershipNumber && u.Id != excludeId.Value)
                : await _context.Users.AnyAsync(u => u.MembershipNumber == membershipNumber);
            return ServiceResult<bool>.Success(exists);
        }
    }
}
