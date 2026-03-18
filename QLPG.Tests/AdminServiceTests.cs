using Microsoft.Extensions.Logging.Abstractions;
using QLPG_a.Models;
using QLPG_a.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace QLPG.Tests
{
    public class AdminServiceTests
    {
        [Fact]
        public async Task GetDashboardAsync_Success_ReturnsValidData()
        {
            var context = TestUtilities.CreateInMemoryContext("admin_dashboard");
            var clock = new FixedDateTimeProvider(new DateTime(2026, 1, 15));
            var service = new AdminService(context, clock, NullLogger<AdminService>.Instance);

            // Seed data
            var user = new User { UserName = "admin", FullName = "Admin User", Phone = "0901234567", Role = "Admin" };
            var member = new Member { MembershipNumber = "M001", FullName = "Member 1", Phone = "0901234567" };
            var goi = new GoiTap { MaGoiTap = "G1", TenGoi = "Package", ThoiHan = 1, Gia = 100000m };
            
            context.Users.Add(user);
            context.Members.Add(member);
            context.GoiTaps.Add(goi);
            context.SaveChanges();

            var reg = new DangKiGoi
            {
                MaDangKy = "DK001",
                MemberId = member.Id,
                GoiTapId = goi.Id,
                NgayBatDau = new DateTime(2026, 1, 1),
                NgayKetThuc = new DateTime(2026, 2, 1),
                TongTien = goi.Gia,
                TrangThai = "Đang hoạt động"
            };
            context.DangKiGois.Add(reg);
            context.SaveChanges();

            var result = await service.GetDashboardAsync();

            Assert.True(result.Succeeded);
            Assert.NotNull(result.Value);
            Assert.Equal(1, result.Value.TotalMembers);
            Assert.Equal(1, result.Value.ActiveSubscriptionsCount);
        }

        [Fact]
        public async Task GetReportsAsync_Success_ReturnsValidData()
        {
            var context = TestUtilities.CreateInMemoryContext("admin_reports");
            var clock = new FixedDateTimeProvider(new DateTime(2026, 1, 15));
            var service = new AdminService(context, clock, NullLogger<AdminService>.Instance);

            // Seed data
            var member = new Member { MembershipNumber = "M001", FullName = "Member 1", Phone = "0901234567" };
            var goi = new GoiTap { MaGoiTap = "G1", TenGoi = "Package", ThoiHan = 1, Gia = 100000m };
            
            context.Members.Add(member);
            context.GoiTaps.Add(goi);
            context.SaveChanges();

            var reg = new DangKiGoi
            {
                MaDangKy = "DK001",
                MemberId = member.Id,
                GoiTapId = goi.Id,
                NgayBatDau = new DateTime(2026, 1, 1),
                NgayKetThuc = new DateTime(2026, 2, 1),
                TongTien = goi.Gia,
                TrangThai = "Đang hoạt động"
            };
            context.DangKiGois.Add(reg);
            context.SaveChanges();

            var result = await service.GetReportsAsync();

            Assert.True(result.Succeeded);
            Assert.NotNull(result.Value);
            Assert.Equal(1, result.Value.ActiveMembers);
        }
    }
}
