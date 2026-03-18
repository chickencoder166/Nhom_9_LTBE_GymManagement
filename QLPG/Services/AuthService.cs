using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QLPG_a.Data;
using QLPG_a.Models;
using QLPG_a.Models.ViewModels;
using System.Threading.Tasks;

namespace QLPG_a.Services
{
    public class AuthService : IAuthService
    {
        private readonly IApplicationDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IApplicationDbContext context,
            IPasswordHasher<User> passwordHasher,
            ILogger<AuthService> logger)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        public async Task<ServiceResult<User>> RegisterAsync(RegisterViewModel model)
        {
            if (model == null)
                return ServiceResult<User>.Fail(ServiceErrorCode.Validation, "Dữ liệu đăng ký không hợp lệ.");

            if (await _context.Users.AnyAsync(u => u.UserName == model.UserName))
                return ServiceResult<User>.Fail(ServiceErrorCode.Conflict, "Tài khoản đã tồn tại.");

            if (await _context.Users.AnyAsync(u => u.MembershipNumber == model.MembershipNumber))
                return ServiceResult<User>.Fail(ServiceErrorCode.Conflict, "Mã người dùng đã tồn tại.");

            var user = new User
            {
                UserName = model.UserName,
                MembershipNumber = model.MembershipNumber,
                FullName = model.FullName,
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender,
                Email = model.Email,
                Phone = model.Phone,
                Role = "Member"
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);

            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return ServiceResult<User>.Success(user);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Lỗi khi đăng ký tài khoản.");
                return ServiceResult<User>.Fail(ServiceErrorCode.Unexpected, "Không thể đăng ký. Vui lòng thử lại.");
            }
        }

        public async Task<ServiceResult<User>> ValidateLoginAsync(LoginViewModel model)
        {
            if (model == null)
                return ServiceResult<User>.Fail(ServiceErrorCode.Validation, "Dữ liệu đăng nhập không hợp lệ.");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == model.UserName);
            if (user == null)
                return ServiceResult<User>.Fail(ServiceErrorCode.NotFound, "Sai tài khoản hoặc mật khẩu.");

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);
            if (result != PasswordVerificationResult.Success)
                return ServiceResult<User>.Fail(ServiceErrorCode.Validation, "Sai tài khoản hoặc mật khẩu.");

            return ServiceResult<User>.Success(user);
        }
    }
}
