using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging.Abstractions;
using QLPG_a.Models.ViewModels;
using QLPG_a.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace QLPG.Tests
{
    public class AuthServiceTests
    {
        [Fact]
        public async Task RegisterAsync_Succeeds_AndHashesPassword()
        {
            var context = TestUtilities.CreateInMemoryContext("auth_register_success");
            var service = new AuthService(context, new PasswordHasher<QLPG_a.Models.User>(), NullLogger<AuthService>.Instance);

            var model = new RegisterViewModel
            {
                MembershipNumber = "U001",
                UserName = "user1",
                Password = "P@ssw0rd!",
                ConfirmPassword = "P@ssw0rd!",
                FullName = "User One",
                DateOfBirth = new DateTime(2000, 1, 1),
                Phone = "0901234567",
                Email = "user1@example.com"
            };

            var result = await service.RegisterAsync(model);

            Assert.True(result.Succeeded);
            Assert.NotNull(result.Value);
            Assert.NotEmpty(result.Value!.PasswordHash);
            Assert.NotEqual(model.Password, result.Value.PasswordHash);
        }

        [Fact]
        public async Task RegisterAsync_Fails_WhenDuplicateUserName()
        {
            var context = TestUtilities.CreateInMemoryContext("auth_register_duplicate");
            var service = new AuthService(context, new PasswordHasher<QLPG_a.Models.User>(), NullLogger<AuthService>.Instance);

            var first = new RegisterViewModel
            {
                MembershipNumber = "U002",
                UserName = "user2",
                Password = "P@ssw0rd!",
                ConfirmPassword = "P@ssw0rd!",
                FullName = "User Two",
                DateOfBirth = new DateTime(2000, 1, 1),
                Phone = "0901234567",
                Email = "user2@example.com"
            };

            var second = new RegisterViewModel
            {
                MembershipNumber = "U003",
                UserName = "user2",
                Password = "P@ssw0rd!",
                ConfirmPassword = "P@ssw0rd!",
                FullName = "User Three",
                DateOfBirth = new DateTime(2000, 1, 1),
                Phone = "0901234567",
                Email = "user3@example.com"
            };

            var ok = await service.RegisterAsync(first);
            var dup = await service.RegisterAsync(second);

            Assert.True(ok.Succeeded);
            Assert.False(dup.Succeeded);
        }

        [Fact]
        public async Task ValidateLoginAsync_Fails_WhenPasswordIncorrect()
        {
            var context = TestUtilities.CreateInMemoryContext("auth_login_invalid");
            var service = new AuthService(context, new PasswordHasher<QLPG_a.Models.User>(), NullLogger<AuthService>.Instance);

            var reg = new RegisterViewModel
            {
                MembershipNumber = "U004",
                UserName = "user4",
                Password = "P@ssw0rd!",
                ConfirmPassword = "P@ssw0rd!",
                FullName = "User Four",
                DateOfBirth = new DateTime(2000, 1, 1),
                Phone = "0901234567"
            };

            await service.RegisterAsync(reg);

            var login = new LoginViewModel { UserName = "user4", Password = "wrong" };
            var result = await service.ValidateLoginAsync(login);

            Assert.False(result.Succeeded);
        }
    }
}
