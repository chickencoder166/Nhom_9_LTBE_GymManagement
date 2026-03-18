using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QLPG_a.Data;
using QLPG_a.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLPG_a.Services
{
    public class DangKiGoiService : IDangKiGoiService
    {
        private readonly IApplicationDbContext _context;
        private readonly IDateTimeProvider _clock;
        private readonly ILogger<DangKiGoiService> _logger;
        private readonly IEmailService _emailService;

        public DangKiGoiService(
            IApplicationDbContext context,
            IDateTimeProvider clock,
            ILogger<DangKiGoiService> logger,
            IEmailService emailService)
        {
            _context = context;
            _clock = clock;
            _logger = logger;
            _emailService = emailService;
        }

        public async Task<ServiceResult<IReadOnlyList<DangKiGoi>>> GetAllAsync()
        {
            var list = await _context.DangKiGois
                .Include(d => d.Member)
                .Include(d => d.GoiTap)
                .AsNoTracking()
                .ToListAsync();
            return ServiceResult<IReadOnlyList<DangKiGoi>>.Success(list);
        }

        public async Task<ServiceResult<DangKiGoi>> GetByIdAsync(int id)
        {
            var item = await _context.DangKiGois
                .Include(d => d.Member)
                .Include(d => d.GoiTap)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);
            return item == null
                ? ServiceResult<DangKiGoi>.Fail(ServiceErrorCode.NotFound, "Đăng ký không tồn tại.")
                : ServiceResult<DangKiGoi>.Success(item);
        }

        public async Task<ServiceResult> CreateAsync(DangKiGoi dangKy)
        {
            if (dangKy == null)
                return ServiceResult.Fail(ServiceErrorCode.Validation, "Dữ liệu đăng ký không hợp lệ.");

            var goi = await _context.GoiTaps.FindAsync(dangKy.GoiTapId);
            if (goi == null)
                return ServiceResult.Fail(ServiceErrorCode.Validation, "Gói tập không tồn tại.");

            dangKy.GoiTap = goi;
            dangKy.CapNhatNgayKetThucBoiGoi();
            dangKy.TongTien = goi.Gia;
            dangKy.TrangThai = dangKy.NgayKetThuc > _clock.Now ? "Đang hoạt động" : "Hết hạn";

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var active = await _context.DangKiGois
                    .Where(d => d.MemberId == dangKy.MemberId && d.TrangThai == "Đang hoạt động" && d.NgayKetThuc > _clock.Now)
                    .ToListAsync();

                foreach (var a in active)
                {
                    a.TrangThai = "Hết hạn";
                    _context.DangKiGois.Update(a);
                }

                _context.DangKiGois.Add(dangKy);
                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                // Send confirmation email (fire-and-forget)
                var member = await _context.Members.FindAsync(dangKy.MemberId);
                if (member != null && !string.IsNullOrWhiteSpace(member.Email))
                {
                    _ = _emailService.SendRegistrationConfirmationAsync(
                        member.Email, member.FullName,
                        goi.TenGoi, dangKy.NgayBatDau, dangKy.NgayKetThuc);
                }

                return ServiceResult.Success();
            }
            catch (DbUpdateException ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Lỗi khi tạo đăng ký gói.");
                return ServiceResult.Fail(ServiceErrorCode.Unexpected, "Lỗi khi lưu đăng ký. Vui lòng thử lại.");
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Lỗi không xác định khi tạo đăng ký gói.");
                return ServiceResult.Fail(ServiceErrorCode.Unexpected, "Lỗi khi lưu đăng ký. Vui lòng thử lại.");
            }
        }

        public async Task<ServiceResult> UpdateAsync(DangKiGoi dangKy)
        {
            if (dangKy == null)
                return ServiceResult.Fail(ServiceErrorCode.Validation, "Dữ liệu đăng ký không hợp lệ.");

            var goi = await _context.GoiTaps.FindAsync(dangKy.GoiTapId);
            if (goi == null)
                return ServiceResult.Fail(ServiceErrorCode.Validation, "Gói tập không tồn tại.");

            dangKy.GoiTap = goi;
            dangKy.CapNhatNgayKetThucBoiGoi();
            dangKy.TongTien = goi.Gia;
            dangKy.TrangThai = dangKy.NgayKetThuc > _clock.Now ? "Đang hoạt động" : "Hết hạn";

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                if (dangKy.TrangThai == "Đang hoạt động")
                {
                    var otherActive = await _context.DangKiGois
                        .Where(d => d.MemberId == dangKy.MemberId && d.Id != dangKy.Id && d.TrangThai == "Đang hoạt động" && d.NgayKetThuc > _clock.Now)
                        .ToListAsync();

                    foreach (var a in otherActive)
                    {
                        a.TrangThai = "Hết hạn";
                        _context.DangKiGois.Update(a);
                    }
                }

                _context.DangKiGois.Update(dangKy);
                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                return ServiceResult.Success();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Xung đột khi cập nhật đăng ký gói.");
                return ServiceResult.Fail(ServiceErrorCode.Concurrency, "Dữ liệu đã bị thay đổi. Vui lòng tải lại.");
            }
            catch (DbUpdateException ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Lỗi khi cập nhật đăng ký gói.");
                return ServiceResult.Fail(ServiceErrorCode.Unexpected, "Lỗi khi cập nhật đăng ký. Vui lòng thử lại.");
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Lỗi không xác định khi cập nhật đăng ký gói.");
                return ServiceResult.Fail(ServiceErrorCode.Unexpected, "Lỗi khi cập nhật đăng ký. Vui lòng thử lại.");
            }
        }

        public async Task<ServiceResult> DeleteAsync(int id)
        {
            var reg = await _context.DangKiGois.FindAsync(id);
            if (reg == null)
                return ServiceResult.Fail(ServiceErrorCode.NotFound, "Đăng ký không tồn tại.");

            try
            {
                _context.DangKiGois.Remove(reg);
                await _context.SaveChangesAsync();
                return ServiceResult.Success();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa đăng ký gói.");
                return ServiceResult.Fail(ServiceErrorCode.Unexpected, "Không thể xóa đăng ký. Vui lòng thử lại.");
            }
        }
    }
}
