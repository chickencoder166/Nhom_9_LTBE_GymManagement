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
    public class HomeService : IHomeService
    {
        private readonly IApplicationDbContext _context;
        private readonly IDateTimeProvider _clock;
        private readonly ILogger<HomeService> _logger;

        public HomeService(IApplicationDbContext context, IDateTimeProvider clock, ILogger<HomeService> logger)
        {
            _context = context;
            _clock = clock;
            _logger = logger;
        }

        public async Task<ServiceResult<HomeSummary>> GetHomeSummaryAsync()
        {
            try
            {
                var totalMembers = await _context.Users.CountAsync();
                var activeMembers = await _context.DangKiGois
                    .Where(d => d.NgayKetThuc > _clock.Now && (d.TrangThai == null || d.TrangThai == "Đang hoạt động"))
                    .Select(d => d.MemberId)
                    .Distinct()
                    .CountAsync();

                var latestNotices = await _context.ThongBaos
                    .OrderByDescending(tb => tb.NgayDang)
                    .Take(5)
                    .ToListAsync();

                return ServiceResult<HomeSummary>.Success(new HomeSummary
                {
                    TotalMembers = totalMembers,
                    ActiveMembers = activeMembers,
                    LatestNotices = latestNotices
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải trang chủ.");
                return ServiceResult<HomeSummary>.Success(new HomeSummary
                {
                    TotalMembers = 0,
                    ActiveMembers = 0,
                    LatestNotices = new List<ThongBao>()
                });
            }
        }

        public async Task<ServiceResult<IReadOnlyList<Member>>> GetMembersAsync(string status)
        {
            try
            {
                var members = await _context.Members.AsNoTracking().ToListAsync();
                if (status == "active")
                {
                    var activeMemberIds = await _context.DangKiGois
                        .Where(d => d.NgayKetThuc > _clock.Now && (d.TrangThai == null || d.TrangThai == "Đang hoạt động"))
                        .Select(d => d.MemberId)
                        .Distinct()
                        .ToListAsync();

                    members = members.Where(m => activeMemberIds.Contains(m.Id)).ToList();
                }
                else if (status == "expired")
                {
                    var expiredMemberIds = await _context.DangKiGois
                        .Where(d => d.NgayKetThuc <= _clock.Now || d.TrangThai == "Hết hạn")
                        .Select(d => d.MemberId)
                        .Distinct()
                        .ToListAsync();

                    members = members.Where(m => expiredMemberIds.Contains(m.Id)).ToList();
                }

                return ServiceResult<IReadOnlyList<Member>>.Success(members);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải danh sách hội viên.");
                return ServiceResult<IReadOnlyList<Member>>.Fail(ServiceErrorCode.Unexpected, "Không thể tải danh sách hội viên.");
            }
        }

        public async Task<ServiceResult<IReadOnlyList<GoiTap>>> GetGoiTapsAsync()
        {
            var list = await _context.GoiTaps.AsNoTracking().OrderBy(s => s.TenGoi).ToListAsync();
            return ServiceResult<IReadOnlyList<GoiTap>>.Success(list);
        }

        public async Task<ServiceResult<IReadOnlyList<ThongBao>>> GetThongBaosAsync()
        {
            var thongBaos = await _context.ThongBaos
                .AsNoTracking()
                .OrderByDescending(tb => tb.NgayDang)
                .ToListAsync();
            return ServiceResult<IReadOnlyList<ThongBao>>.Success(thongBaos);
        }

        public async Task<ServiceResult<MemberDashboardViewModel>> GetMemberDashboardAsync(string userName)
        {
            try
            {
                var user = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.UserName == userName);

                if (user == null)
                    return ServiceResult<MemberDashboardViewModel>.Fail(ServiceErrorCode.NotFound, "Không tìm thấy tài khoản.");

                // Find the Member entity matching this user's MembershipNumber
                var member = await _context.Members
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.MembershipNumber == user.MembershipNumber);

                if (member == null)
                {
                    // No linked member entity — return dashboard with user info only
                    var vmEmpty = new MemberDashboardViewModel
                    {
                        FullName = user.FullName,
                        Email = user.Email,
                        AvatarPath = user.AvatarPath,
                        MembershipNumber = user.MembershipNumber,
                        AvailablePackages = await _context.GoiTaps.AsNoTracking().OrderBy(g => g.ThoiHan).ToListAsync()
                    };
                    return ServiceResult<MemberDashboardViewModel>.Success(vmEmpty);
                }

                // Get all subscriptions for this member
                var history = await _context.DangKiGois
                    .Include(d => d.GoiTap)
                    .AsNoTracking()
                    .Where(d => d.MemberId == member.Id)
                    .OrderByDescending(d => d.NgayBatDau)
                    .ToListAsync();

                var active = history.FirstOrDefault(d =>
                    d.TrangThai == "Đang hoạt động" && d.NgayKetThuc > _clock.Now);

                var packages = await _context.GoiTaps.AsNoTracking().OrderBy(g => g.ThoiHan).ToListAsync();

                var vm = new MemberDashboardViewModel
                {
                    FullName = user.FullName,
                    Email = user.Email,
                    AvatarPath = user.AvatarPath,
                    MembershipNumber = user.MembershipNumber,
                    ActiveSubscription = active,
                    History = history,
                    AvailablePackages = packages
                };

                return ServiceResult<MemberDashboardViewModel>.Success(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải dashboard hội viên.");
                return ServiceResult<MemberDashboardViewModel>.Fail(ServiceErrorCode.Unexpected, "Không thể tải thông tin hội viên.");
            }
        }
    }
}
