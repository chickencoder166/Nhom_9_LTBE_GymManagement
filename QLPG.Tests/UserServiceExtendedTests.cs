using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging.Abstractions;
using QLPG_a.Models;
using QLPG_a.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace QLPG.Tests
{
    public class UserServiceExtendedTests
    {
        [Fact]
        public async Task DeleteAsync_Success_WithValidId()
        {
            var context = TestUtilities.CreateInMemoryContext("user_delete");
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

            var createResult = await service.CreateAsync(user, "P@ssw0rd!");
            Assert.True(createResult.Succeeded);

            var idToDelete = createResult.Value?.Id ?? 0;
            var deleteResult = await service.DeleteAsync(idToDelete);

            Assert.True(deleteResult.Succeeded);
            Assert.Null(context.Users.Find(idToDelete));
        }

        [Fact]
        public async Task DeleteAsync_Fails_WithInvalidId()
        {
            var context = TestUtilities.CreateInMemoryContext("user_delete_invalid");
            var service = new UserService(context, new PasswordHasher<User>(), NullLogger<UserService>.Instance);

            var result = await service.DeleteAsync(999);

            Assert.False(result.Succeeded);
        }

        [Fact]
        public async Task GetByIdAsync_Success_ReturnsUser()
        {
            var context = TestUtilities.CreateInMemoryContext("user_getbyid");
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

            var createResult = await service.CreateAsync(user, "P@ssw0rd!");
            Assert.True(createResult.Succeeded);

            int userId = createResult.Value?.Id ?? 0;
            var getResult = await service.GetByIdAsync(userId);

            Assert.True(getResult.Succeeded);
            Assert.NotNull(getResult.Value);
            Assert.Equal("user1", getResult.Value.UserName);
        }
    }
}
