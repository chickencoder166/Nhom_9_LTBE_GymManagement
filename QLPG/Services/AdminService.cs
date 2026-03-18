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
    public class AdminService : IAdminService
    {
        private readonly IApplicationDbContext _context;
        private readonly IDateTimeProvider _clock;
        private readonly ILogger<AdminService> _logger;

        public AdminService(IApplicationDbContext context, IDateTimeProvider clock, ILogger<AdminService> logger)
        {
            _context = context;
            _clock = clock;
            _logger = logger;
        }

        public async Task<ServiceResult<DashboardViewModel>> GetDashboardAsync()
        {
            try
            {
                var totalUsers = await _context.Users.CountAsync();
                var totalMembers = await _context.Members.CountAsync();

                var activeDangKiGoiTaps = await _context.DangKiGois
                    .Where(d => d.NgayKetThuc > _clock.Now && (d.TrangThai == null || d.TrangThai == "Đang hoạt động"))
                    .CountAsync();

                var expiredMembersCount = await _context.DangKiGois
                    .Where(d => d.NgayKetThuc <= _clock.Now || d.TrangThai == "Hết hạn")
                    .Select(d => d.MemberId)
                    .Distinct()
                    .CountAsync();

                var cutoff = _clock.Now.AddMonths(-6);
                var revenues = await _context.DangKiGois
                    .Where(d => d.NgayBatDau >= cutoff)
                    .GroupBy(d => new { d.NgayBatDau.Year, d.NgayBatDau.Month })
                    .Select(g => new MonthlyRevenue { Year = g.Key.Year, Month = g.Key.Month, Total = g.Sum(x => x.TongTien) })
                    .OrderBy(r => r.Year).ThenBy(r => r.Month)
                    .ToListAsync();

                var vm = new DashboardViewModel
                {
                    TotalUsers = totalUsers,
                    TotalMembers = totalMembers,
                    ActiveSubscriptionsCount = activeDangKiGoiTaps,
                    ExpiredMembersCount = expiredMembersCount,
                    Revenues = revenues
                };

                return ServiceResult<DashboardViewModel>.Success(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo Dashboard.");
                return ServiceResult<DashboardViewModel>.Fail(ServiceErrorCode.Unexpected, "Không thể tải Dashboard.");
            }
        }

        public async Task<ServiceResult<ReportsViewModel>> GetReportsAsync()
        {
            try
            {
                var from = _clock.Now.AddMonths(-11);
                var revenues = await _context.DangKiGois
                    .Where(d => d.NgayBatDau >= from)
                    .GroupBy(d => new { d.NgayBatDau.Year, d.NgayBatDau.Month })
                    .Select(g => new MonthlyRevenue { Year = g.Key.Year, Month = g.Key.Month, Total = g.Sum(x => x.TongTien) })
                    .OrderBy(r => r.Year).ThenBy(r => r.Month)
                    .ToListAsync();

                var activeMembersCount = await _context.DangKiGois
                    .Where(d => d.NgayKetThuc > _clock.Now && (d.TrangThai == null || d.TrangThai == "Đang hoạt động"))
                    .Select(d => d.MemberId)
                    .Distinct()
                    .CountAsync();

                var expiredMembersCount = await _context.DangKiGois
                    .Where(d => d.NgayKetThuc <= _clock.Now || d.TrangThai == "Hết hạn")
                    .Select(d => d.MemberId)
                    .Distinct()
                    .CountAsync();

                var topPackages = await _context.DangKiGois
                    .Include(d => d.GoiTap)
                    .GroupBy(d => d.GoiTap!.TenGoi)
                    .Select(g => new TopPackageStat
                    {
                        PackageName = g.Key,
                        RegistrationCount = g.Count(),
                        Revenue = g.Sum(x => x.TongTien)
                    })
                    .OrderByDescending(x => x.RegistrationCount)
                    .ThenByDescending(x => x.Revenue)
                    .Take(3)
                    .ToListAsync();

                var vm = new ReportsViewModel
                {
                    Revenues = revenues,
                    ActiveMembers = activeMembersCount,
                    ExpiredMembers = expiredMembersCount,
                    TopPackages = topPackages
                };

                return ServiceResult<ReportsViewModel>.Success(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo báo cáo.");
                return ServiceResult<ReportsViewModel>.Fail(ServiceErrorCode.Unexpected, "Không thể tải báo cáo.");
            }
        }

        public async Task<ServiceResult<IReadOnlyList<GoiTap>>> GetAllGoiTapsAsync()
        {
            var list = await _context.GoiTaps.AsNoTracking().OrderBy(s => s.TenGoi).ToListAsync();
            return ServiceResult<IReadOnlyList<GoiTap>>.Success(list);
        }

        public async Task<ServiceResult<IReadOnlyList<MemberViewModel>>> GetMembersAsync(string status, string? query = null)
        {
            var members = await _context.Members.AsNoTracking().ToListAsync();
            var latestSubscriptions = await _context.DangKiGois
                .Include(d => d.GoiTap)
                .AsNoTracking()
                .OrderByDescending(d => d.NgayBatDau)
                .ToListAsync();

            var projected = members.Select(member =>
            {
                var current = latestSubscriptions.FirstOrDefault(d => d.MemberId == member.Id && d.TrangThai == "Đang hoạt động" && d.NgayKetThuc > _clock.Now)
                             ?? latestSubscriptions.FirstOrDefault(d => d.MemberId == member.Id);

                var statusValue = current == null
                    ? "Chưa đăng ký"
                    : (current.NgayKetThuc > _clock.Now && (current.TrangThai == null || current.TrangThai == "Đang hoạt động")
                        ? "Đang hoạt động"
                        : "Hết hạn");

                return new MemberViewModel
                {
                    Id = member.Id,
                    MembershipNumber = member.MembershipNumber ?? string.Empty,
                    FullName = member.FullName,
                    Phone = member.Phone ?? string.Empty,
                    Email = member.Email,
                    CurrentPackageName = current?.GoiTap?.TenGoi ?? member.Package,
                    StartDate = current?.NgayBatDau,
                    EndDate = current?.NgayKetThuc,
                    Status = statusValue,
                    RemainingDays = current != null ? (int)Math.Ceiling((current.NgayKetThuc.Date - _clock.Now.Date).TotalDays) : null
                };
            });

            if (status == "active")
            {
                projected = projected.Where(m => m.Status == "Đang hoạt động");
            }
            else if (status == "expired")
            {
                projected = projected.Where(m => m.Status == "Hết hạn");
            }

            if (!string.IsNullOrWhiteSpace(query))
            {
                projected = projected.Where(m =>
                    m.FullName.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    m.Phone.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    m.MembershipNumber.Contains(query, StringComparison.OrdinalIgnoreCase));
            }

            return ServiceResult<IReadOnlyList<MemberViewModel>>.Success(projected.OrderBy(m => m.FullName).ToList());
        }
    }
}
