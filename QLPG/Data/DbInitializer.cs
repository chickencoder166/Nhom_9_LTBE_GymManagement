using QLPG_a.Data;
using QLPG_a.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic;

namespace QLPG_a.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Ensure database exists
            context.Database.EnsureCreated();

            // If we already have users or subscriptions, assume seeded
            if (context.Users.Any() || context.Subcriptions.Any())
            {
                return;
            }

            // 1) Seed Subcriptions (5 plans)
            var subs = new List<GoiTap>
            {
                new GoiTap { MaGoiTap = "P1M", TenGoi = "Gói 1 tháng", ThoiHan = 1, Gia = 500000, MoTa = "Gói 1 tháng" },
                new GoiTap { MaGoiTap = "P3M", TenGoi = "Gói 3 tháng", ThoiHan = 3, Gia = 1400000, MoTa = "Gói 3 tháng" },
                new GoiTap { MaGoiTap = "P6M", TenGoi = "Gói 6 tháng", ThoiHan = 6, Gia = 2600000, MoTa = "Gói 6 tháng" },
                new GoiTap { MaGoiTap = "P12M", TenGoi = "Gói 12 tháng", ThoiHan = 12, Gia = 4800000, MoTa = "Gói 12 tháng" },
                new GoiTap { MaGoiTap = "P2M", TenGoi = "Gói 2 tháng", ThoiHan = 2, Gia = 900000, MoTa = "Gói 2 tháng" }
            };
            context.Subcriptions.AddRange(subs);
            context.SaveChanges();

            // 2) Seed Users (50 members + 2 admins)
            var users = new List<User>();
            users.Add(new User {
                MembershipNumber = "A0001",
                UserName = "admin",
                PasswordHash = "admin",
                FullName = "Administrator",
                DateOfBirth = DateTime.Now.AddYears(-30),
                Role = "Admin",
                Phone = "0900000000",
                Email = "admin@qlpg.local"
            });
            // create 50 members
            for (int i = 1; i <= 50; i++)
            {
                users.Add(new User
                {
                    MembershipNumber = $"M{i:000}",
                    UserName = $"member{i:000}",
                    PasswordHash = "password",
                    FullName = $"Hội viên {i}",
                    DateOfBirth = DateTime.Now.AddYears(-20).AddDays(i),
                    Role = "Member",
                    Phone = $"09{10000000 + i}",
                    Email = $"member{i}@example.com"
                });
            }
            context.Users.AddRange(users);
            context.SaveChanges();

            // 3) Seed DangKyGoi: 70 registrations (45 active, 25 expired)
            var rnd = new Random(12345);
            var allUsers = context.Users.Where(u => u.Role == "Member").ToList();
            var allSubs = context.Subcriptions.ToList();

            var registrations = new List<DangKyGoi>();

            // First ensure 45 distinct users get an active subscription
            var activeUsers = allUsers.OrderBy(u => rnd.Next()).Take(45).ToList();
            int dkIndex = 1;
            foreach (var u in activeUsers)
            {
                var sub = allSubs[rnd.Next(allSubs.Count)];
                var start = DateTime.Now.AddDays(-rnd.Next(0, 20)); // started within last 20 days
                var end = start.AddDays(sub.ThoiHan * 30);
                registrations.Add(new DangKyGoi
                {
                    MaDangKy = $"DK{dkIndex:000}",
                    UserId = u.Id,
                    SubcriptionId = sub.Id,
                    NgayBatDau = start,
                    NgayKetThuc = end,
                    TongTien = sub.Gia,
                    TrangThai = "Đang hoạt động"
                });
                dkIndex++;
            }

            // Then create 25 expired registrations (can be for any users)
            for (int i = 0; i < 25; i++)
            {
                var u = allUsers[rnd.Next(allUsers.Count)];
                var sub = allSubs[rnd.Next(allSubs.Count)];
                var start = DateTime.Now.AddDays(-rnd.Next(60, 365));
                var end = start.AddDays(sub.ThoiHan * 30);
                registrations.Add(new DangKyGoi
                {
                    MaDangKy = $"DK{dkIndex:000}",
                    UserId = u.Id,
                    SubcriptionId = sub.Id,
                    NgayBatDau = start,
                    NgayKetThuc = end,
                    TongTien = sub.Gia,
                    TrangThai = "Hết hạn"
                });
                dkIndex++;
            }

            context.DangKyGois.AddRange(registrations);
            context.SaveChanges();

            // 4) Create unique filtered index to ensure each user has at most one active subscription
            try
            {
                var createIndexSql = @"
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'UX_ActivePerUser' AND object_id = OBJECT_ID('dbo.DangKyGois'))
BEGIN
    CREATE UNIQUE INDEX UX_ActivePerUser ON dbo.DangKyGois(UserId) WHERE TrangThai = 'Đang hoạt động';
END";
                context.Database.ExecuteSqlRaw(createIndexSql);
            }
            catch
            {
                // ignore index creation errors on providers that don't support filtered indexes
            }

            // 5) Create trigger to mark previous active subscriptions as expired when inserting new one
            try
            {
                var createTriggerSql = @"
IF OBJECT_ID('dbo.tri_ExpireOldOnInsert','TR') IS NULL
BEGIN
    EXEC('CREATE TRIGGER dbo.tri_ExpireOldOnInsert ON dbo.DangKyGois AFTER INSERT AS\nBEGIN\n    SET NOCOUNT ON;\n    UPDATE d\n    SET d.TrangThai = ''Hết hạn''\n    FROM dbo.DangKyGois d\n    INNER JOIN inserted i ON d.UserId = i.UserId\n    WHERE d.Id <> i.Id AND (d.TrangThai IS NULL OR d.TrangThai = ''Đang hoạt động'');\nEND')
END";
                context.Database.ExecuteSqlRaw(createTriggerSql);
            }
            catch
            {
                // ignore trigger creation errors on unsupported providers
            }
        }

        internal static void ApplyMigrations(ApplicationDbContext context)
        {
            throw new NotImplementedException();
        }
    }
}
