using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QLPG_a.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QLPG_a.Services
{
    public class EfCrudService<TEntity> : ICrudService<TEntity> where TEntity : class
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<EfCrudService<TEntity>> _logger;

        public EfCrudService(IApplicationDbContext context, ILogger<EfCrudService<TEntity>> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ServiceResult<IReadOnlyList<TEntity>>> GetAllAsync()
        {
            var list = await _context.Set<TEntity>().AsNoTracking().ToListAsync();
            return ServiceResult<IReadOnlyList<TEntity>>.Success(list);
        }

        public async Task<ServiceResult<TEntity>> GetByIdAsync(int id)
        {
            var entity = await _context.Set<TEntity>().FindAsync(id);
            return entity == null
                ? ServiceResult<TEntity>.Fail(ServiceErrorCode.NotFound, "Dữ liệu không tồn tại.")
                : ServiceResult<TEntity>.Success(entity);
        }

        public async Task<ServiceResult<TEntity>> CreateAsync(TEntity entity)
        {
            if (entity == null)
                return ServiceResult<TEntity>.Fail(ServiceErrorCode.Validation, "Dữ liệu không hợp lệ.");

            try
            {
                _context.Set<TEntity>().Add(entity);
                await _context.SaveChangesAsync();
                return ServiceResult<TEntity>.Success(entity);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo dữ liệu.");
                return ServiceResult<TEntity>.Fail(ServiceErrorCode.Unexpected, "Không thể tạo dữ liệu. Vui lòng thử lại.");
            }
        }

        public async Task<ServiceResult> UpdateAsync(TEntity entity)
        {
            if (entity == null)
                return ServiceResult.Fail(ServiceErrorCode.Validation, "Dữ liệu không hợp lệ.");

            try
            {
                _context.Set<TEntity>().Update(entity);
                await _context.SaveChangesAsync();
                return ServiceResult.Success();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Lỗi xung đột khi cập nhật dữ liệu.");
                return ServiceResult.Fail(ServiceErrorCode.Concurrency, "Dữ liệu đã bị thay đổi. Vui lòng tải lại.");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật dữ liệu.");
                return ServiceResult.Fail(ServiceErrorCode.Unexpected, "Không thể cập nhật dữ liệu. Vui lòng thử lại.");
            }
        }

        public async Task<ServiceResult> DeleteAsync(int id)
        {
            var entity = await _context.Set<TEntity>().FindAsync(id);
            if (entity == null)
                return ServiceResult.Fail(ServiceErrorCode.NotFound, "Dữ liệu không tồn tại.");

            try
            {
                _context.Set<TEntity>().Remove(entity);
                await _context.SaveChangesAsync();
                return ServiceResult.Success();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa dữ liệu.");
                return ServiceResult.Fail(ServiceErrorCode.Unexpected, "Không thể xóa dữ liệu. Vui lòng thử lại.");
            }
        }
    }
}
