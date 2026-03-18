using QLPG_a.Models;
using QLPG_a.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QLPG_a.Services
{
    public interface IHomeService
    {
        Task<ServiceResult<HomeSummary>> GetHomeSummaryAsync();
        Task<ServiceResult<IReadOnlyList<Member>>> GetMembersAsync(string status);
        Task<ServiceResult<IReadOnlyList<GoiTap>>> GetGoiTapsAsync();
        Task<ServiceResult<IReadOnlyList<ThongBao>>> GetThongBaosAsync();
        Task<ServiceResult<MemberDashboardViewModel>> GetMemberDashboardAsync(string userName);
    }

    public class HomeSummary
    {
        public int TotalMembers { get; init; }
        public int ActiveMembers { get; init; }
        public IReadOnlyList<ThongBao> LatestNotices { get; init; } = new List<ThongBao>();
    }
}
