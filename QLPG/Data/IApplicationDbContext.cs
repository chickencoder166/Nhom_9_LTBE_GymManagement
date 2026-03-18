using Microsoft.EntityFrameworkCore;
using QLPG_a.Models;
using System.Threading;
using System.Threading.Tasks;

namespace QLPG_a.Data
{
    public interface IApplicationDbContext
    {
        DbSet<User> Users { get; set; }
        DbSet<Member> Members { get; set; }
        DbSet<GoiTap> GoiTaps { get; set; }
        DbSet<DangKiGoi> DangKiGois { get; set; }
        DbSet<ThongBao> ThongBaos { get; set; }

        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        int SaveChanges();
        Microsoft.EntityFrameworkCore.Infrastructure.DatabaseFacade Database { get; }
    }
}
