using QLPG_a.Data;
using QLPG_a.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace QLPG_a.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.Migrate();

            // Check if data already seeded
            if (context.Members.Any())
            {
                return;
            }

            var today = DateTime.Today;

            // Seed GoiTaps (plans/packages)
            var goiTaps = new List<GoiTap>
            {
                new GoiTap
                {
                    MaGoiTap = "GOI_1M",
                    TenGoi = "Gói 1 tháng",
                    ThoiHan = 1,
                    Gia = 100000m,
                    MoTa = "Gói cơ bản 1 tháng"
                },
                new GoiTap
                {
                    MaGoiTap = "GOI_3M",
                    TenGoi = "Gói 3 tháng",
                    ThoiHan = 3,
                    Gia = 250000m,
                    MoTa = "Gói tiết kiệm 3 tháng"
                },
                new GoiTap
                {
                    MaGoiTap = "GOI_6M",
                    TenGoi = "Gói 6 tháng",
                    ThoiHan = 6,
                    Gia = 500000m,
                    MoTa = "Gói nâng cao 6 tháng"
                },
                new GoiTap
                {
                    MaGoiTap = "GOI_12M",
                    TenGoi = "Gói 12 tháng",
                    ThoiHan = 12,
                    Gia = 900000m,
                    MoTa = "Gói cả năm tiết kiệm"
                },
                new GoiTap
                {
                    MaGoiTap = "GOI_PT",
                    TenGoi = "Gói PT 1 tháng",
                    ThoiHan = 1,
                    Gia = 250000m,
                    MoTa = "Gói có huấn luyện viên cá nhân"
                }
            };

            context.GoiTaps.AddRange(goiTaps);
            context.SaveChanges();

            var members = Enumerable.Range(1, 50)
                .Select(i => new Member
                {
                    MembershipNumber = $"M{i:000}",
                    FullName = $"Hội viên {i:000}",
                    DateOfBirth = new DateTime(1990, 1, 1).AddDays(i * 45),
                    Email = $"member{i:000}@gym.local",
                    Phone = $"09{i:00000000}",
                    JoinDate = today.AddDays(-i * 3),
                    Package = null
                })
                .ToList();

            context.Members.AddRange(members);
            context.SaveChanges();

            var registrations = new List<DangKiGoi>();

            // 45 active registrations
            for (int i = 0; i < 45; i++)
            {
                var plan = goiTaps[i % goiTaps.Count];
                var startDate = today.AddDays(-(i % 20));
                var registration = new DangKiGoi
                {
                    MaDangKy = $"DK{i + 1:000}",
                    MemberId = members[i].Id,
                    GoiTapId = plan.Id,
                    NgayBatDau = startDate,
                    NgayKetThuc = startDate.AddDays(plan.ThoiHan * 30),
                    TongTien = plan.Gia,
                    TrangThai = "Đang hoạt động"
                };

                members[i].Package = plan.TenGoi;
                registrations.Add(registration);
            }

            // 25 expired registrations (history for first 25 members)
            for (int i = 0; i < 25; i++)
            {
                var plan = goiTaps[(i + 1) % goiTaps.Count];
                var startDate = today.AddDays(-((plan.ThoiHan * 30) + 45 + i));
                registrations.Add(new DangKiGoi
                {
                    MaDangKy = $"DK{45 + i + 1:000}",
                    MemberId = members[i].Id,
                    GoiTapId = plan.Id,
                    NgayBatDau = startDate,
                    NgayKetThuc = startDate.AddDays(plan.ThoiHan * 30),
                    TongTien = plan.Gia,
                    TrangThai = "Hết hạn"
                });
            }

            context.DangKiGois.AddRange(registrations);
            context.SaveChanges();

            // Seed admin/member user accounts for demo login
            if (!context.Users.Any())
            {
                var hasher = new PasswordHasher<User>();

                var adminUser = new User
                {
                    MembershipNumber = "ADMIN001",
                    UserName = "admin",
                    FullName = "Gym Admin",
                    DateOfBirth = new DateTime(1990, 1, 1),
                    Gender = "Nam",
                    Email = "admin@gym.local",
                    Phone = "0900000000",
                    Role = "Admin"
                };
                adminUser.PasswordHash = hasher.HashPassword(adminUser, "Admin123");

                var memberUser = new User
                {
                    MembershipNumber = members[0].MembershipNumber ?? "M001",
                    UserName = "member001",
                    FullName = members[0].FullName,
                    DateOfBirth = members[0].DateOfBirth ?? new DateTime(1995, 1, 1),
                    Gender = "Nữ",
                    Email = members[0].Email,
                    Phone = members[0].Phone ?? "0901111111",
                    Role = "Member"
                };
                memberUser.PasswordHash = hasher.HashPassword(memberUser, "Member123");

                context.Users.AddRange(
                    adminUser,
                    memberUser);

                context.SaveChanges();
            }
        }

        public static void ApplyMigrations(ApplicationDbContext context)
        {
            try
            {
                context.Database.Migrate();
            }
            catch
            {
                // Database creation may fail if unavailable
            }
        }
    }
}
