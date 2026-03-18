using Microsoft.Extensions.Logging.Abstractions;
using QLPG_a.Models;
using QLPG_a.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace QLPG.Tests
{
    public class DangKiGoiServiceConcurrencyTests
    {
        [Fact]
        public async Task DeleteAsync_Success_WithValidId()
        {
            var context = TestUtilities.CreateInMemoryContext("dangki_delete");
            var clock = new FixedDateTimeProvider(new DateTime(2026, 1, 1));
            var emailService = new NullEmailService();
            var service = new DangKiGoiService(context, clock, NullLogger<DangKiGoiService>.Instance, emailService);

            var member = new Member { FullName = "Member 1", Phone = "0901234567" };
            var goi = new GoiTap { MaGoiTap = "G1", TenGoi = "Basic", ThoiHan = 1, Gia = 100000m };
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

            int idToDelete = reg.Id;
            var result = await service.DeleteAsync(idToDelete);

            Assert.True(result.Succeeded);
            Assert.Null(context.DangKiGois.Find(idToDelete));
        }

        [Fact]
        public async Task UpdateAsync_Success_WithValidData()
        {
            var context = TestUtilities.CreateInMemoryContext("dangki_update_valid");
            var clock = new FixedDateTimeProvider(new DateTime(2026, 1, 1));
            var emailService = new NullEmailService();
            var service = new DangKiGoiService(context, clock, NullLogger<DangKiGoiService>.Instance, emailService);

            var member = new Member { FullName = "Member 1", Phone = "0901234567" };
            var goi = new GoiTap { MaGoiTap = "G1", TenGoi = "Basic", ThoiHan = 1, Gia = 100000m };
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

            // Update existing registration
            reg.NgayBatDau = new DateTime(2026, 1, 5);
            var result = await service.UpdateAsync(reg);

            Assert.True(result.Succeeded);
            var updated = context.DangKiGois.Find(reg.Id);
            Assert.Equal("Đang hoạt động", updated?.TrangThai);
            Assert.Equal(new DateTime(2026, 1, 5), updated?.NgayBatDau);
        }
    }
}
