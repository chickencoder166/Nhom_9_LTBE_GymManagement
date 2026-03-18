using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging.Abstractions;
using QLPG_a.Models;
using QLPG_a.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace QLPG.Tests
{
    public class UserServiceTests
    {
        [Fact]
        public async Task CreateAsync_Fails_WhenPasswordMissing()
        {
            var context = TestUtilities.CreateInMemoryContext("user_create_no_pwd");
            var service = new UserService(context, new PasswordHasher<User>(), NullLogger<UserService>.Instance);

            var user = new User
            {
                MembershipNumber = "M001",
                UserName = "user1",
                FullName = "User One",
                DateOfBirth = new DateTime(2000, 1, 1),
                Phone = "0901234567",
                Role = "Member"
            };

            var result = await service.CreateAsync(user, null);

            Assert.False(result.Succeeded);
        }

        [Fact]
        public async Task CreateAsync_Fails_WhenDuplicateUserName()
        {
            var context = TestUtilities.CreateInMemoryContext("user_create_dup");
            var service = new UserService(context, new PasswordHasher<User>(), NullLogger<UserService>.Instance);

            var user1 = new User
            {
                MembershipNumber = "M001",
                UserName = "user1",
                FullName = "User One",
                DateOfBirth = new DateTime(2000, 1, 1),
                Phone = "0901234567",
                Role = "Member"
            };

            var user2 = new User
            {
                MembershipNumber = "M002",
                UserName = "user1",
                FullName = "User Two",
                DateOfBirth = new DateTime(2000, 1, 1),
                Phone = "0901234567",
                Role = "Member"
            };

            var ok = await service.CreateAsync(user1, "P@ssw0rd!");
            var dup = await service.CreateAsync(user2, "P@ssw0rd!");

            Assert.True(ok.Succeeded);
            Assert.False(dup.Succeeded);
        }

        [Fact]
        public async Task UpdateAsync_Fails_WhenDuplicateMembershipNumber()
        {
            var context = TestUtilities.CreateInMemoryContext("user_update_dup_membership");
            var service = new UserService(context, new PasswordHasher<User>(), NullLogger<UserService>.Instance);

            var user1 = new User
            {
                MembershipNumber = "M001",
                UserName = "user1",
                FullName = "User One",
                DateOfBirth = new DateTime(2000, 1, 1),
                Phone = "0901234567",
                Role = "Member"
            };

            var user2 = new User
            {
                MembershipNumber = "M002",
                UserName = "user2",
                FullName = "User Two",
                DateOfBirth = new DateTime(2000, 1, 1),
                Phone = "0901234567",
                Role = "Member"
            };

            var ok1 = await service.CreateAsync(user1, "P@ssw0rd!");
            var ok2 = await service.CreateAsync(user2, "P@ssw0rd!");

            var updated = new User
            {
                Id = ok2.Value!.Id,
                MembershipNumber = "M001",
                UserName = ok2.Value.UserName,
                FullName = ok2.Value.FullName,
                DateOfBirth = ok2.Value.DateOfBirth,
                Phone = ok2.Value.Phone,
                Role = ok2.Value.Role
            };

            var result = await service.UpdateAsync(updated);

            Assert.False(result.Succeeded);
        }
    }
}
