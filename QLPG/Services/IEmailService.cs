namespace QLPG_a.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string htmlBody);
        Task SendRegistrationConfirmationAsync(string toEmail, string fullName, string packageName, DateTime startDate, DateTime endDate);
        Task SendPasswordResetAsync(string toEmail, string fullName, string resetLink);
    }
}
