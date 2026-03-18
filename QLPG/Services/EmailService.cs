using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace QLPG_a.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            try
            {
                var host = _config["Email:SmtpHost"] ?? "smtp.gmail.com";
                var port = int.Parse(_config["Email:SmtpPort"] ?? "587");
                var user = _config["Email:SmtpUser"] ?? "";
                var pass = _config["Email:SmtpPass"] ?? "";
                var from = _config["Email:FromAddress"] ?? user;
                var fromName = _config["Email:FromName"] ?? "QLPG Gym";

                if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass))
                {
                    _logger.LogWarning("Email chưa được cấu hình. Bỏ qua gửi email tới {Email}", toEmail);
                    return;
                }

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(fromName, from));
                message.To.Add(MailboxAddress.Parse(toEmail));
                message.Subject = subject;
                message.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = htmlBody };

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(host, port, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(user, pass);
                await smtp.SendAsync(message);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gửi email tới {Email}", toEmail);
            }
        }

        public async Task SendRegistrationConfirmationAsync(string toEmail, string fullName, string packageName,
            DateTime startDate, DateTime endDate)
        {
            var subject = "Xác nhận đăng ký gói tập – QLPG Gym";
            var html = $@"
<div style='font-family:Arial,sans-serif;max-width:600px;margin:0 auto;'>
  <h2 style='color:#dc3545;'>🏋️ QLPG Gym – Xác nhận đăng ký</h2>
  <p>Xin chào <strong>{fullName}</strong>,</p>
  <p>Bạn đã đăng ký thành công gói tập tại <strong>QLPG Gym</strong>.</p>
  <table style='border-collapse:collapse;width:100%;'>
    <tr><td style='padding:8px;border:1px solid #ddd;background:#f8f9fa;'><b>Gói tập</b></td>
        <td style='padding:8px;border:1px solid #ddd;'>{packageName}</td></tr>
    <tr><td style='padding:8px;border:1px solid #ddd;background:#f8f9fa;'><b>Ngày bắt đầu</b></td>
        <td style='padding:8px;border:1px solid #ddd;'>{startDate:dd/MM/yyyy}</td></tr>
    <tr><td style='padding:8px;border:1px solid #ddd;background:#f8f9fa;'><b>Ngày kết thúc</b></td>
        <td style='padding:8px;border:1px solid #ddd;'>{endDate:dd/MM/yyyy}</td></tr>
  </table>
  <p style='margin-top:20px;'>Cảm ơn bạn đã tham gia QLPG Gym!</p>
  <hr/>
  <small style='color:#6c757d;'>Email này được gửi tự động, vui lòng không trả lời.</small>
</div>";
            await SendEmailAsync(toEmail, subject, html);
        }

        public async Task SendPasswordResetAsync(string toEmail, string fullName, string resetLink)
        {
            var subject = "Đặt lại mật khẩu – QLPG Gym";
            var html = $@"
<div style='font-family:Arial,sans-serif;max-width:600px;margin:0 auto;'>
  <h2 style='color:#dc3545;'>🔑 QLPG Gym – Đặt lại mật khẩu</h2>
  <p>Xin chào <strong>{fullName}</strong>,</p>
  <p>Chúng tôi nhận được yêu cầu đặt lại mật khẩu cho tài khoản của bạn.</p>
  <p>Nhấn vào nút bên dưới để đặt lại mật khẩu (liên kết có hiệu lực trong 30 phút):</p>
  <p style='text-align:center;'>
    <a href='{resetLink}' style='background:#dc3545;color:#fff;padding:12px 24px;
       text-decoration:none;border-radius:4px;font-weight:bold;'>Đặt lại mật khẩu</a>
  </p>
  <p>Nếu bạn không yêu cầu, hãy bỏ qua email này.</p>
  <hr/>
  <small style='color:#6c757d;'>Email này được gửi tự động, vui lòng không trả lời.</small>
</div>";
            await SendEmailAsync(toEmail, subject, html);
        }
    }
}
