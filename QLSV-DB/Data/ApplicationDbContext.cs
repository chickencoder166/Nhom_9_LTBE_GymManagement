using Microsoft.EntityFrameworkCore;
using QLPG_a.Models;

namespace QLPG_a.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {
        }

        // Gym-related DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Subcription> Subcriptions { get; set; }
        public DbSet<DangKyGoi> DangKyGois { get; set; }
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

            // Subcription (Gói tập)
            modelBuilder.Entity<Subcription>(entity =>
            {
                entity.HasKey(e => e.Id);
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

            // DangKyGoi (Đăng ký gói)
            modelBuilder.Entity<DangKyGoi>(entity =>
            {
                entity.HasKey(e => e.Id);

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

                // Relationship with User/Member
                entity.HasOne(d => d.User)
                    .WithMany()
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relationship with Subcription
                entity.HasOne(d => d.Subcription)
                    .WithMany(s => s.DangKyGois)
                    .HasForeignKey(d => d.SubcriptionId)
                    .OnDelete(DeleteBehavior.Restrict);
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
