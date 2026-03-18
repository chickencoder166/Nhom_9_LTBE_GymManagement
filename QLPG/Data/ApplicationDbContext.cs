using Microsoft.EntityFrameworkCore;
using QLPG_a.Models;

namespace QLPG_a.Data
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Gym-related DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<GoiTap> GoiTaps { get; set; }
        public DbSet<DangKiGoi> DangKiGois { get; set; }
        public DbSet<ThongBao> ThongBaos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.MembershipNumber)
                    .IsRequired()
                    .HasMaxLength(10);
                // Unique index on MembershipNumber (users)
                entity.HasIndex(e => e.MembershipNumber).IsUnique();
                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.HasIndex(e => e.UserName).IsUnique();
                entity.Property(e => e.PasswordHash)
                    .IsRequired();
                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(e => e.DateOfBirth);
                entity.Property(e => e.Gender)
                    .HasMaxLength(20);
                entity.Property(e => e.Email)
                    .HasMaxLength(100);
                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(20);
                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasMaxLength(20);
            });

            // Member
            modelBuilder.Entity<Member>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.MembershipNumber).HasMaxLength(20);
                entity.Property(e => e.FullName).HasMaxLength(200);
                entity.HasIndex(e => e.MembershipNumber);
            });

            // GoiTap (gói tập)
            modelBuilder.Entity<GoiTap>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("GoiTaps");
                entity.Property(e => e.MaGoiTap)
                    .IsRequired()
                    .HasMaxLength(20);
                entity.Property(e => e.TenGoi)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(e => e.ThoiHan)
                    .IsRequired();
                entity.Property(e => e.Gia)
                    .HasColumnType("decimal(18,2)");
                entity.Property(e => e.MoTa)
                    .HasMaxLength(1000);
            });

            // DangKiGoi (đăng ký gói)
            modelBuilder.Entity<DangKiGoi>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("DangKiGois");

                entity.Property(e => e.MaDangKy)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.NgayBatDau)
                    .IsRequired();
                entity.Property(e => e.NgayKetThuc)
                    .IsRequired();

                entity.Property(e => e.TongTien)
                    .HasColumnType("decimal(18,2)");
                entity.Property(e => e.TrangThai)
                    .HasMaxLength(50);

                // Concurrency token (RowVersion)
                entity.Property<byte[]>("RowVersion").IsRowVersion();

                // Relationship with Member
                entity.HasOne(d => d.Member)
                    .WithMany()
                    .HasForeignKey(d => d.MemberId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relationship with GoiTap
                entity.HasOne(d => d.GoiTap)
                    .WithMany(p => p.DangKiGois)
                    .HasForeignKey(d => d.GoiTapId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(d => d.MemberId);
                entity.HasIndex(d => d.GoiTapId);
            });

            // ThongBao
            modelBuilder.Entity<ThongBao>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TieuDe)
                    .IsRequired()
                    .HasMaxLength(500);
                entity.Property(e => e.NoiDung)
                    .HasMaxLength(2000);
                entity.Property(e => e.LoaiThongBao)
                    .HasMaxLength(50);
                entity.Property(e => e.NgayDang)
                    .HasDefaultValueSql("GETDATE()");

                entity.HasIndex(e => e.LoaiThongBao);
            });
        }
    }
}
