using Microsoft.Extensions.Logging.Abstractions;
using QLPG_a.Models;
using QLPG_a.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace QLPG.Tests
{
    public class LookupServiceTests
    {
        [Fact]
        public async Task GetGoiTapsAsync_Returns_AllPackages()
        {
            var context = TestUtilities.CreateInMemoryContext("lookup_goitaps");
            var service = new LookupService(context);

            var goi1 = new GoiTap { MaGoiTap = "G1", TenGoi = "Package 1", ThoiHan = 1, Gia = 100000m };
            var goi2 = new GoiTap { MaGoiTap = "G2", TenGoi = "Package 2", ThoiHan = 3, Gia = 250000m };
            
            context.GoiTaps.Add(goi1);
            context.GoiTaps.Add(goi2);
            context.SaveChanges();

            var result = await service.GetGoiTapsAsync();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetMembersAsync_Returns_AllMembers()
        {
            var context = TestUtilities.CreateInMemoryContext("lookup_members");
            var service = new LookupService(context);

            var member1 = new Member { MembershipNumber = "M001", FullName = "Member 1", Phone = "0901234567" };
            var member2 = new Member { MembershipNumber = "M002", FullName = "Member 2", Phone = "0909876543" };
            
            context.Members.Add(member1);
            context.Members.Add(member2);
            context.SaveChanges();

            var result = await service.GetMembersAsync();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetGoiTapsAsync_Returns_Empty_WhenNoPackages()
        {
            var context = TestUtilities.CreateInMemoryContext("lookup_goitaps_empty");
            var service = new LookupService(context);

            var result = await service.GetGoiTapsAsync();

            Assert.Empty(result);
        }
    }
}
