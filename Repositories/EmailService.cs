using System.Net.Mail;

using MimeKit;
using SeleafAPI.Interfaces;

namespace SeleafAPI.Repositories
{
    public class EmailService : IEmailSender
    {
        private readonly EmailConfiguration _emailConfig;
        private readonly ILogger<EmailService> _logger;


        public EmailService(EmailConfiguration emailConfig, ILogger<EmailService> logger)
        {
            _emailConfig = emailConfig;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string info)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailConfig.FromName, _emailConfig.FromAddress));
            message.To.Add(new MailboxAddress("", to));
            message.Subject = subject;

            message.Body = new TextPart("html")
            {
                Text = info
            };

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    _logger.LogInformation("Connecting to SMTP server...");
                    await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, MailKit.Security.SecureSocketOptions.SslOnConnect);
                    _logger.LogInformation("Connected to SMTP server");

                    _logger.LogInformation("Authenticating...");
                    await client.AuthenticateAsync(_emailConfig.UserName, _emailConfig.Password);
                    _logger.LogInformation("Authenticated");

                    _logger.LogInformation("Sending email...");
                    await client.SendAsync(message);
                    _logger.LogInformation("Email sent successfully");

                    await client.DisconnectAsync(true);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while sending email");
                    throw;
                }
            }
        }

        public async Task SendEmailAsyncWithAttachment(string to, string subject, string info, MemoryStream pdfStream)
        {
            // Validate the recipient email address
            try
            {
                var mailAddress = new MailAddress(to);
            }
            catch (FormatException)
            {
                _logger.LogError("Invalid email address: {to}", to);
                throw new ArgumentException("Invalid email address format.");
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailConfig.FromName, _emailConfig.FromAddress));
            message.To.Add(new MailboxAddress("", to));  // Validated email address is added here
            message.Subject = subject;

            var body = new TextPart("html")
            {
                Text = info
            };

            var attachment = new MimePart("application", "pdf")
            {
                Content = new MimeContent(pdfStream, ContentEncoding.Default),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = "Section 18A.pdf"
            };

            var multipart = new Multipart("mixed");
            multipart.Add(body);
            multipart.Add(attachment);

            message.Body = multipart;

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    _logger.LogInformation("Connecting to SMTP server...");
                    await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, MailKit.Security.SecureSocketOptions.SslOnConnect);
                    _logger.LogInformation("Connected to SMTP server");

                    _logger.LogInformation("Authenticating...");
                    await client.AuthenticateAsync(_emailConfig.UserName, _emailConfig.Password);
                    _logger.LogInformation("Authenticated");

                    _logger.LogInformation("Sending email...");
                    await client.SendAsync(message);
                    _logger.LogInformation("Email sent successfully");

                    await client.DisconnectAsync(true);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while sending email");
                    throw;
                }
            }
        }
    }
    public class EmailConfiguration
    {
        public string? FromName { get; set; }
        public string? FromAddress { get; set; }
        public string? SmtpServer { get; set; }
        public int Port { get; set; }
        public bool UseSsl { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
    }
}
