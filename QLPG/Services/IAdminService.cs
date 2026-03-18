using QLPG_a.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QLPG_a.Services
{
    public interface IAdminService
    {
        Task<ServiceResult<DashboardViewModel>> GetDashboardAsync();
        Task<ServiceResult<ReportsViewModel>> GetReportsAsync();
        Task<ServiceResult<IReadOnlyList<QLPG_a.Models.GoiTap>>> GetAllGoiTapsAsync();
        Task<ServiceResult<IReadOnlyList<MemberViewModel>>> GetMembersAsync(string status, string? query = null);
    }
}
