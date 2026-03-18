using System.Collections.Generic;
using System.Threading.Tasks;

namespace QLPG_a.Services
{
    public interface ICrudService<TEntity> where TEntity : class
    {
        Task<ServiceResult<IReadOnlyList<TEntity>>> GetAllAsync();
        Task<ServiceResult<TEntity>> GetByIdAsync(int id);
        Task<ServiceResult<TEntity>> CreateAsync(TEntity entity);
        Task<ServiceResult> UpdateAsync(TEntity entity);
        Task<ServiceResult> DeleteAsync(int id);
    }
}
