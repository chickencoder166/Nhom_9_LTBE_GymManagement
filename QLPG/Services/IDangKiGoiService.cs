using QLPG_a.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QLPG_a.Services
{
    public interface IDangKiGoiService
    {
        Task<ServiceResult<IReadOnlyList<DangKiGoi>>> GetAllAsync();
        Task<ServiceResult<DangKiGoi>> GetByIdAsync(int id);
        Task<ServiceResult> CreateAsync(DangKiGoi dangKy);
        Task<ServiceResult> UpdateAsync(DangKiGoi dangKy);
        Task<ServiceResult> DeleteAsync(int id);
    }
}
