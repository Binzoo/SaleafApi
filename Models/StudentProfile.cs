using System.ComponentModel.DataAnnotations.Schema;

namespace SeleafAPI.Models;

public class StudentProfile
{
    public string? Id { get; set; }

    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    [ForeignKey("AppUser")]
    public string? AppUserId { get; set; }


    public string? Skills { get; set; }
    public List<string>? Achievements { get; set; }
    public string? Year { get; set; }
    public bool IsFinalYear { get; set; }
    public string? Bio { get; set; }
    public DateTime? GraduationDate { get; set; }
    public string? University { get; set; }
    public string? Degree { get; set; }
    public string? OnlineProfile { get; set; }
}