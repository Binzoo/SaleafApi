using Microsoft.Build.Framework;

namespace SeleafAPI.Models.DTO;

public class StudentProfileDTO
{
    [Required]
    public string? FirstName { get; set; }
    [Required]
    public string? LastName { get; set; }
    public string? Skills { get; set; }
    public List<string>? Achievements { get; set; }

    [Required]
    public string? Year { get; set; }
    public bool IsFinalYear { get; set; }
    public string? Bio { get; set; }
    public DateTime? GraduationDate { get; set; } 
    public string? University { get; set; }
    public string? Degree { get; set; }
    public string? OnlineProfile { get; set; }
}