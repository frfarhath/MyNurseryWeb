using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using DnsClient;

namespace MyNursery.Services
{
    public class EmailSettings
    {
        public string From { get; set; } = string.Empty;
        public string SmtpServer { get; set; } = string.Empty;
        public int Port { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IOptions<EmailSettings> emailSettings, ILogger<EmailSender> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        private bool IsValidEmailFormat(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> DomainHasMxRecordsAsync(string email)
        {
            try
            {
                var domain = email.Split('@')[1];
                var lookup = new LookupClient();
                var result = await lookup.QueryAsync(domain, QueryType.MX);
                return result.Answers.MxRecords().Any();
            }
            catch
            {
                return false;
            }
        }


        public async Task<string> SendEmailWithValidationAsync(string email, string subject, string htmlMessage)
        {
            if (!IsValidEmailFormat(email))
            {
                _logger.LogWarning($"Invalid email format: {email}");
                return "Invalid email format.";
            }

            if (!await DomainHasMxRecordsAsync(email))
            {
                _logger.LogWarning($"Email domain does not have MX records: {email}");
                return "Email domain is not accepting emails.";
            }

            try
            {
                using var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.Port)
                {
                    Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.From, "Little Sprouts Nursery"),
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(email);
                await client.SendMailAsync(mailMessage);
                _logger.LogInformation($"Email sent to {email} with subject {subject}");
                return "Success";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {email}");
                return $"Failed to send email: {ex.Message}";
            }
        }

        // For backward compatibility with Identity UI (OTP / Reset)
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            await SendEmailWithValidationAsync(email, subject, htmlMessage);
        }
    }
}