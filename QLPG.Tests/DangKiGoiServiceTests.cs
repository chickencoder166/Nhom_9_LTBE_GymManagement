using Microsoft.Extensions.Logging.Abstractions;
using QLPG_a.Models;
using QLPG_a.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace QLPG.Tests
{
    public class DangKiGoiServiceTests
    {
        [Fact]
        public async Task CreateAsync_Fails_WhenGoiTapMissing()
        {
            var context = TestUtilities.CreateInMemoryContext("dangky_create_missing_goi");
            var clock = new FixedDateTimeProvider(new DateTime(2026, 1, 1));
            var emailService = new NullEmailService();
            var service = new DangKiGoiService(context, clock, NullLogger<DangKiGoiService>.Instance, emailService);

            var reg = new DangKiGoi
            {
                MaDangKy = "DK001",
                MemberId = 1,
                GoiTapId = 999,
                NgayBatDau = new DateTime(2026, 1, 1)
            };

            var result = await service.CreateAsync(reg);

            Assert.False(result.Succeeded);
        }

        [Fact]
        public async Task CreateAsync_ExpiresExistingActiveSubscriptions()
        {
            var context = TestUtilities.CreateInMemoryContext("dangky_create_expires");
            var clock = new FixedDateTimeProvider(new DateTime(2026, 1, 1));
            var emailService = new NullEmailService();
            var service = new DangKiGoiService(context, clock, NullLogger<DangKiGoiService>.Instance, emailService);

            var member = new Member { FullName = "Member 1", Phone = "0901234567" };
            var goi = new GoiTap { MaGoiTap = "G001", TenGoi = "Basic", ThoiHan = 4, Gia = 100000m };
            context.Members.Add(member);
            context.GoiTaps.Add(goi);
            context.SaveChanges();

            var existing = new DangKiGoi
            {
                MaDangKy = "DK001",
                MemberId = member.Id,
                GoiTapId = goi.Id,
                NgayBatDau = new DateTime(2025, 12, 1),
                NgayKetThuc = new DateTime(2026, 2, 1),
                TongTien = goi.Gia,
                TrangThai = "Đang hoạt động"
            };
            context.DangKiGois.Add(existing);
            context.SaveChanges();

            var newReg = new DangKiGoi
            {
                MaDangKy = "DK002",
                MemberId = member.Id,
                GoiTapId = goi.Id,
                NgayBatDau = new DateTime(2026, 1, 1)
            };

            var result = await service.CreateAsync(newReg);

            Assert.True(result.Succeeded);
            Assert.Equal("Hết hạn", existing.TrangThai);
        }

        [Fact]
        public async Task UpdateAsync_Fails_WhenGoiTapMissing()
        {
            var context = TestUtilities.CreateInMemoryContext("dangky_update_missing_goi");
            var clock = new FixedDateTimeProvider(new DateTime(2026, 1, 1));
            var emailService = new NullEmailService();
            var service = new DangKiGoiService(context, clock, NullLogger<DangKiGoiService>.Instance, emailService);

            var reg = new DangKiGoi
            {
                Id = 1,
                MaDangKy = "DK001",
                MemberId = 1,
                GoiTapId = 999,
                NgayBatDau = new DateTime(2026, 1, 1)
            };

            var result = await service.UpdateAsync(reg);

            Assert.False(result.Succeeded);
        }
    }
}
