using QLPG_a.Models;
using QLPG_a.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QLPG_a.Services
{
    public interface IUserService
    {
        Task<ServiceResult<IReadOnlyList<User>>> GetAllAsync();
        Task<ServiceResult<User>> GetByIdAsync(int id);
        Task<ServiceResult<User>> CreateAsync(User user, string? plainPassword);
        Task<ServiceResult> UpdateAsync(User user);
        Task<ServiceResult> DeleteAsync(int id);
        Task<ServiceResult<IReadOnlyList<MemberViewModel>>> SearchMembersAsync(string? query, string? status = null);
        Task<ServiceResult<bool>> UserNameExistsAsync(string userName, int? excludeId = null);
        Task<ServiceResult<bool>> MembershipNumberExistsAsync(string membershipNumber, int? excludeId = null);
    }
}
