using Microsoft.Extensions.Logging.Abstractions;
using QLPG_a.Models;
using QLPG_a.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace QLPG.Tests
{
    public class GoiTapServiceTests
    {
        [Fact]
        public async Task CreateAsync_Success_WithValidData()
        {
            var context = TestUtilities.CreateInMemoryContext("goitap_create_valid");
            var service = new EfCrudService<GoiTap>(context, NullLogger<EfCrudService<GoiTap>>.Instance);

            var goi = new GoiTap
            {
                MaGoiTap = "GOI_NEW",
                TenGoi = "New Package",
                ThoiHan = 3,
                Gia = 250000m,
                MoTa = "Test package"
            };

            var result = await service.CreateAsync(goi);

            Assert.True(result.Succeeded);
            Assert.NotNull(result.Value);
            Assert.True(result.Value.Id > 0);
        }

        [Fact]
        public async Task GetAllAsync_Returns_AllPackages()
        {
            var context = TestUtilities.CreateInMemoryContext("goitap_getall");
            var service = new EfCrudService<GoiTap>(context, NullLogger<EfCrudService<GoiTap>>.Instance);

            // Add test data
            var goi1 = new GoiTap { MaGoiTap = "G1", TenGoi = "Package 1", ThoiHan = 1, Gia = 100000m };
            var goi2 = new GoiTap { MaGoiTap = "G2", TenGoi = "Package 2", ThoiHan = 3, Gia = 250000m };
            
            context.GoiTaps.Add(goi1);
            context.GoiTaps.Add(goi2);
            context.SaveChanges();

            var result = await service.GetAllAsync();

            Assert.True(result.Succeeded);
            Assert.Equal(2, result.Value?.Count ?? 0);
        }

        [Fact]
        public async Task UpdateAsync_Success_WithValidData()
        {
            var context = TestUtilities.CreateInMemoryContext("goitap_update");
            var service = new EfCrudService<GoiTap>(context, NullLogger<EfCrudService<GoiTap>>.Instance);

            var goi = new GoiTap { MaGoiTap = "G1", TenGoi = "Original", ThoiHan = 1, Gia = 100000m };
            context.GoiTaps.Add(goi);
            context.SaveChanges();

            goi.TenGoi = "Updated";
            goi.Gia = 120000m;

            var result = await service.UpdateAsync(goi);

            Assert.True(result.Succeeded);
            
            var updated = context.GoiTaps.Find(goi.Id);
            Assert.Equal("Updated", updated?.TenGoi);
            Assert.Equal(120000m, updated?.Gia);
        }

        [Fact]
        public async Task DeleteAsync_Success_WithValidId()
        {
            var context = TestUtilities.CreateInMemoryContext("goitap_delete");
            var service = new EfCrudService<GoiTap>(context, NullLogger<EfCrudService<GoiTap>>.Instance);

            var goi = new GoiTap { MaGoiTap = "G1", TenGoi = "Package 1", ThoiHan = 1, Gia = 100000m };
            context.GoiTaps.Add(goi);
            context.SaveChanges();

            int idToDelete = goi.Id;

            var result = await service.DeleteAsync(idToDelete);

            Assert.True(result.Succeeded);
            Assert.Null(context.GoiTaps.Find(idToDelete));
        }

        [Fact]
        public async Task DeleteAsync_Fails_WithInvalidId()
        {
            var context = TestUtilities.CreateInMemoryContext("goitap_delete_invalid");
            var service = new EfCrudService<GoiTap>(context, NullLogger<EfCrudService<GoiTap>>.Instance);

            var result = await service.DeleteAsync(999);

            Assert.False(result.Succeeded);
        }
    }
}
