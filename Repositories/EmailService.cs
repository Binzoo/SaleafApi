using MailKit.Net.Smtp;
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
            message.To.Add(new MailboxAddress("", to)); // Empty string for display name
            message.Subject = subject;

            string body = @"
                            <html>
                            <head>
                                <style>
                                    .email-content {
                                        font-family: Arial, sans-serif;
                                        color: #333;
                                    }
                                    .email-header {
                                        background-color: #f8f8f8;
                                        padding: 20px;
                                        text-align: center;
                                        border-bottom: 1px solid #ddd;
                                    }
                                    .email-body {
                                        padding: 20px;
                                    }
                                    .email-footer {
                                        background-color: #f8f8f8;
                                        padding: 10px;
                                        text-align: center;
                                        font-size: 12px;
                                        color: #888;
                                        border-top: 1px solid #ddd;
                                    }
                                    .button {
                                        background-color: #28a745;
                                        color: white;
                                        padding: 10px 20px;
                                        text-align: center;
                                        text-decoration: none;
                                        display: inline-block;
                                        border-radius: 5px;
                                    }
                                </style>
                            </head>
                            <body>
                                <div class='email-content'>
                                    <div class='email-header'>
                                        <h1>Thank You for Your Donation!</h1>
                                    </div>
                                    <div class='email-body'>
                                        <p>Dear valued supporter,</p>
                                        <p>We sincerely appreciate your generous donation.</p>
                                        <p>Your support helps us continue our work and make a positive impact.</p>
                                        <p>If you have any questions, feel free to <a href='#'>contact us</a>.</p>
                                        <p>Best regards,<br/>The SALEAF Team</p>
                                        <a href='#' class='button'>View Your Donation</a>
                                    </div>
                                    <div class='email-footer'>
                                        <p>&copy; 2024 SALEAF. All rights reserved.</p>
                                    </div>
                                </div>
                            </body>
                            </html>";

            message.Body = new TextPart("html")
            {
                Text = body
            };

            using (var client = new SmtpClient())
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
    }
    public class EmailConfiguration
    {
        public string FromName { get; set; }
        public string FromAddress { get; set; }
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public bool UseSsl { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }

}
