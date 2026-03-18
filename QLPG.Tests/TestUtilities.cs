using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using QLPG_a.Data;
using QLPG_a.Services;
using System;
using System.Threading.Tasks;

namespace QLPG.Tests
{
    internal static class TestUtilities
    {
        public static ApplicationDbContext CreateInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(dbName)
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            return new ApplicationDbContext(options);
        }
    }

    internal sealed class FixedDateTimeProvider : IDateTimeProvider
    {
        public FixedDateTimeProvider(DateTime now)
        {
            Now = now;
        }

        public DateTime Now { get; }
    }

    /// <summary>
    /// Null object for email service in tests - does nothing
    /// </summary>
    internal sealed class NullEmailService : IEmailService
    {
        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            await Task.CompletedTask;
        }

        public async Task SendRegistrationConfirmationAsync(string toEmail, string fullName, string packageName, DateTime startDate, DateTime endDate)
        {
            await Task.CompletedTask;
        }

        public async Task SendPasswordResetAsync(string toEmail, string fullName, string resetLink)
        {
            await Task.CompletedTask;
        }
    }
}

