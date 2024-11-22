namespace SeleafAPI.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string to, string subject, string body);
        Task SendEmailAsyncWithAttachment(string toEmail, string subject, string bodyText, MemoryStream pdfStream, string attachmentName);
    }

}
