using SeleafAPI.Data;
using SeleafAPI.Interfaces;

namespace SeleafAPI.Services;

public class StudentNotificationJob
{
    private readonly IEmailSender _emailService;
    private readonly AppDbContext _context;

    public StudentNotificationJob(IEmailSender emailService, AppDbContext context)
    {
        _emailService = emailService;
        _context = context;
    }
    
    public async Task NotifyStudentsAsync()
    {
        var students = _context.Users.Where(u => u.isStudent == true).Where(v => v.isVerified == true).ToList();
        var currentMonth = DateTime.Now.Month;
        string period = currentMonth == 7 ? "July" : "December";

        foreach (var student in students)
        {
            var emailBody = $"<p>Dear {student.FirstName},</p>" +
                            $"<p>Please upload your student report for the {period} session before {period} 9.</p>" +
                            "<p>Best Regards,<br>Saleaf Team</p>";

            await _emailService.SendEmailAsync(student.Email, $"{period} Report Submission Reminder", emailBody);
        }
    }
}