using QLPG_a.Data;
using QLPG_a.Models;
using System;
using System.Linq;
using System.Collections.Generic;

namespace QLPG_a.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Members.Any())
            {
                return; // DB has been seeded
            }

            // Seed Membership Plans (Subcriptions)
            var subs = new[]
            {
                new Subcription { MaGoiTap = "PLAN_BASIC", TenGoi = "Basic Plan", ThoiHan = 1, Gia = 100, MoTa = "Basic membership" },
                new Subcription { MaGoiTap = "PLAN_PRO", TenGoi = "Pro Plan", ThoiHan = 3, Gia = 250, MoTa = "Pro membership" }
            };
            foreach (var s in subs) context.Subcriptions.Add(s);
            context.SaveChanges();

            // Seed Members
            var members = new Member[]
            {
                new Member { MembershipNumber = "M001", FullName = "Nguyễn Văn An", DateOfBirth = new DateTime(1995, 1, 15), MembershipPlan = "PLAN_BASIC", Email = "nvan@example.com", Phone = "0901234567", JoinDate = DateTime.Now.AddMonths(-6) },
                new Member { MembershipNumber = "M002", FullName = "Trần Thị Bình", DateOfBirth = new DateTime(1993, 3, 20), MembershipPlan = "PLAN_PRO", Email = "ttbinh@example.com", Phone = "0901234568", JoinDate = DateTime.Now.AddMonths(-3) }
            };
            foreach (var m in members) context.Members.Add(m);
            context.SaveChanges();

            // Seed sample DangKyGoi linking members to subscriptions
            var dk = new DangKyGoi[]
            {
                new DangKyGoi { MaDangKy = "DK001", UserId = members[0].Id, SubcriptionId = subs[0].Id, NgayBatDau = DateTime.Now.AddMonths(-5), NgayKetThuc = DateTime.Now.AddMonths(-5).AddMonths(subs[0].ThoiHan), TongTien = subs[0].Gia, TrangThai = "Hết hạn" },
                new DangKyGoi { MaDangKy = "DK002", UserId = members[1].Id, SubcriptionId = subs[1].Id, NgayBatDau = DateTime.Now.AddMonths(-2), NgayKetThuc = DateTime.Now.AddMonths(-2).AddMonths(subs[1].ThoiHan), TongTien = subs[1].Gia, TrangThai = "Đang hoạt động" }
            };
            foreach (var d in dk) context.DangKyGois.Add(d);
            context.SaveChanges();
        }

        internal static void ApplyMigrations(ApplicationDbContext context)
        {
            throw new NotImplementedException();
        }
    }
}
